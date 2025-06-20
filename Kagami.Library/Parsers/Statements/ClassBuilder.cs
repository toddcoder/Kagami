﻿using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.CommonFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class ClassBuilder
{
   protected string className;
   protected Parameters parameters;
   protected string parentClassName;
   protected Expression[] parentArguments;
   protected bool initialize;
   protected Block constructorBlock;
   protected Hash<string, (ConstructorInvokable, Block)> constructorInvokables = [];
   protected List<(IInvokable, Block, bool)> functions = [];
   protected UserClass userClass = new("", "");
   protected Set<Selector> requiredFunctions = [];

   public ClassBuilder(string className, Parameters parameters, string parentClassName, Expression[] parentArguments,
      bool initialize, Block constructorBlock)
   {
      this.className = className;
      this.parameters = parameters;
      this.parentClassName = parentClassName;
      this.parentArguments = parentArguments;
      this.initialize = initialize;
      this.constructorBlock = constructorBlock;
   }

   public Optional<Unit> Register()
   {
      userClass = new UserClass(className, parentClassName);
      var _result = Module.Global.Value.RegisterClass(userClass);
      if (_result)
      {
         return Constructor(parameters, constructorBlock, true);
      }
      else
      {
         return _result.Exception;
      }
   }

   public UserClass UserClass => userClass;

   public Statement[] Statements { get; set; } = [];

   protected static bool isPrivate(string identifier) => identifier.IsMatch("^ '_' -(> '_$')");

   protected (string, Expression)[] getInitializeArguments()
   {
      return parentArguments.Select(e => e.Symbols[0]).Cast<NameValueSymbol>().Select(nv => nv.Tuple()).ToArray();
   }

   protected Block modifyBlock(Block originalBlock, bool standard)
   {
      userClass.RegisterParameters(parameters);

      List<Statement> statements = [];

      if (parentClassName.IsNotEmpty())
      {
         if (Module.Global.Value.Class(parentClassName) is (true, var baseClass))
         {
            var parentClass = (UserClass)baseClass;
            if (standard)
            {
               userClass.InheritFrom(parentClass);
            }

            Symbol symbol = initialize ? new InitializeParentConstructorSymbol(parentClassName, getInitializeArguments())
               : new InvokeParentConstructorSymbol(parentClassName, parentArguments, false);
            statements.Add(new ExpressionStatement(symbol, false));
         }
         else
         {
            throw classNotFound(parentClassName);
         }
      }

      foreach (var statement in originalBlock)
      {
         switch (statement)
         {
            case AssignToNewField assignToNewField:
            {
               var (mutable, fieldName, _) = assignToNewField;
               var function = Function.Getter(fieldName);
               statements.Add(function);
               var (functionName, _, block, _, invokable, _) = function;
               if (!isPrivate(fieldName) && !userClass.RegisterMethod(functionName, new Lambda(invokable), true))
               {
                  throw needsOverride(functionName);
               }

               functions.Add((invokable, block, true));

               if (mutable)
               {
                  function = Function.Setter(fieldName);
                  statements.Add(function);
                  (functionName, _, block, _, invokable, _) = function;
                  if (!isPrivate(fieldName) && !userClass.RegisterMethod(functionName, new Lambda(invokable), true))
                  {
                     throw needsOverride(functionName);
                  }

                  functions.Add((invokable, block, true));
               }

               statements.Add(statement);
               break;
            }
            case AssignToNewField2 assignToNewField2:
            {
               var (comparisand, _) = assignToNewField2;
               if (comparisand.Symbols[0] is PlaceholderSymbol placeholder)
               {
                  var fieldName = placeholder.Name;
                  var (bindingType, name) = fromBindingName(fieldName);
                  var function = Function.Getter(name);
                  statements.Add(function);
                  var (functionName, _, block, _, invokable, _) = function;
                  if (!isPrivate(fieldName) && !userClass.RegisterMethod(functionName, new Lambda(invokable), true))
                  {
                     throw needsOverride(functionName);
                  }

                  functions.Add((invokable, block, true));

                  if (bindingType == BindingType.Mutable)
                  {
                     function = Function.Setter(fieldName);
                     statements.Add(function);
                     (functionName, _, block, _, invokable, _) = function;
                     if (!isPrivate(fieldName) && !userClass.RegisterMethod(functionName, new Lambda(invokable), true))
                     {
                        throw needsOverride(functionName);
                     }

                     functions.Add((invokable, block, true));
                  }

                  statements.Add(statement);
               }

               break;
            }
            case Function function when standard:
            {
               var (selector, _, block, _, invokable, overriding) = function;
               if (!isPrivate(selector))
               {
                  if (userClass.RegisterMethod(selector, new Lambda(invokable), overriding))
                  {
                     functions.Add((invokable, block, overriding));
                  }
                  else
                  {
                     throw needsOverride(selector);
                  }
               }

               if (requiredFunctions.Contains(selector))
               {
                  requiredFunctions.Remove(selector);
               }

               statements.Add(statement);
               break;
            }
            case MatchFunction matchFunction when standard:
            {
               var (functionName, _, block, _, invokable, overriding) = matchFunction;
               if (!isPrivate(functionName))
               {
                  if (userClass.RegisterMethod(functionName, new Lambda(invokable), overriding))
                  {
                     functions.Add((invokable, block, overriding));
                  }
                  else
                  {
                     throw needsOverride(functionName);
                  }
               }

               statements.Add(statement);
               break;
            }
            case RequiredFunction requiredFunction:
            {
               requiredFunctions.Add(requiredFunction.Selector);
               userClass.RegisterInclusion(requiredFunction.Inclusion);
               break;
            }
            default:
               statements.Add(statement);
               break;
         }
      }

      if (requiredFunctions.Count > 0)
      {
         var functionList = requiredFunctions.ToString(", ");
         throw fail(requiredFunctions.Count.Plural($"Required function(s) {functionList} not implemented"));
      }

      statements.Add(new ReturnNewObject(className, parameters));

      Statements = statements.ToArray();

      return new Block(statements);
   }

   public Optional<Unit> Constructor(Parameters parameters, Block block, bool standard)
   {
      var invokable = new ConstructorInvokable(className, parameters);
      var fullFunctionName = parameters.Selector(className);
      if (constructorInvokables.ContainsKey(fullFunctionName))
      {
         return fail($"Constructor {fullFunctionName} already exists");
      }
      else
      {
         constructorInvokables[fullFunctionName] = (invokable, modifyBlock(block, standard));
         return unit;
      }
   }

   public void Generate(OperationsBuilder builder, int index)
   {
      foreach (var (key, value) in constructorInvokables)
      {
         Selector selector = key;
         var (invokable, block) = value;
         var _index = builder.RegisterInvokable(invokable, block, true);
         if (!_index)
         {
            throw _index.Exception;
         }

         builder.NewSelector(selector, false, true);
         builder.PushObject(new Constructor(invokable));
         builder.Peek(index);
         builder.AssignSelector(selector, true);
      }

      foreach (var function in functions)
      {
         var (invokable, block, overriding) = function;
         var _index = builder.RegisterInvokable(invokable, block, overriding);
         if (!_index)
         {
            throw _index.Exception;
         }
      }
   }

   public override string ToString()
   {
      return $"class {className}({parameters}){parentClassName.Map(s => $"{s} of ({parentArguments.ToString(", ")})")}";
   }
}