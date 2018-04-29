using System.Collections.Generic;
using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using Standard.Types.Strings;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ClassBuilder
   {
      string className;
      Parameters parameters;
      string parentClassName;
      Expression[] parentArguments;
      Block constructorBlock;
      Hash<string, (ConstructorInvokable, Block)> constructorInvokables;
      List<(IInvokable, Block, bool)> functions;
      UserClass userClass;

      public ClassBuilder(string className, Parameters parameters, string parentClassName, Expression[] parentArguments,
         Block constructorBlock)
      {
         this.className = className;
         this.parameters = parameters;
         this.parentClassName = parentClassName;
         this.parentArguments = parentArguments;
         this.constructorBlock = constructorBlock;
         constructorInvokables = new Hash<string, (ConstructorInvokable, Block)>();
         functions = new List<(IInvokable, Block, bool)>();
      }

      public IMatched<Unit> Register()
      {
         userClass = new UserClass(className, parentClassName);
         if (Module.Global.RegisterClass(userClass).IfNot(out var exception))
            return failedMatch<Unit>(exception);
         else
            return Constructor(parameters, constructorBlock, true);
      }

      public UserClass UserClass => userClass;

      public Statement[] Statements { get; set; } = new Statement[0];

      static bool isPrivate(string identifier) => identifier.IsMatch("^ '_' -(> '_$')");

      Block modifyBlock(Block originalBlock, bool standard)
      {
         userClass.RegisterParameters(parameters);

         var statements = new List<Statement>();

         if (parentClassName.IsNotEmpty())
            if (Module.Global.Class(parentClassName).If(out var baseClass))
            {
               var parentClass = (UserClass)baseClass;
               if (standard)
                  userClass.InheritFrom(parentClass);
               statements.Add(new ExpressionStatement(new InvokeParentConstructorSymbol(parentClassName, parentArguments), false));
            }
            else
               throw classNotFound(parentClassName);

         foreach (var statement in originalBlock)
            switch (statement)
            {
               case AssignToNewField2 assignToNewField:
               {
                  var (mutable, comparisand, _) = assignToNewField;
                  if (comparisand.Symbols[0] is PlaceholderSymbol placeholder)
                  {
                     var fieldName = placeholder.Name;
                     var function = Function.Getter(fieldName);
                     statements.Add(function);
                     var (functionName, _, block, _, invokable, _) = function;
                     if (!isPrivate(fieldName) && !userClass.RegisterMethod(functionName, new Lambda(invokable), true))
                        throw needsOverride(functionName);

                     functions.Add((invokable, block, true));

                     if (mutable)
                     {
                        function = Function.Setter(fieldName);
                        statements.Add(function);
                        (functionName, _, block, _, invokable, _) = function;
                        if (!isPrivate(fieldName) && !userClass.RegisterMethod(functionName, new Lambda(invokable), true))
                           throw needsOverride(functionName);

                        functions.Add((invokable, block, true));
                     }

                     statements.Add(statement);
                  }
               }
                  break;
               case Function function when standard:
               {
                  var (functionName, funcParameters, block, _, invokable, overriding) = function;
                  functionName = funcParameters.FullFunctionName(functionName);
                  if (!isPrivate(functionName))
                     if (userClass.RegisterMethod(functionName, new Lambda(invokable), overriding))
                        functions.Add((invokable, block, overriding));
                     else
                        throw needsOverride(functionName);

                  statements.Add(statement);
               }
                  break;
               case MatchFunction matchFunction when standard:
               {
                  var (functionName, funcParameters, block, _, invokable, overriding) = matchFunction;
                  functionName = funcParameters.FullFunctionName(functionName);
                  if (!isPrivate(functionName))
                     if (userClass.RegisterMethod(functionName, new Lambda(invokable), overriding))
                        functions.Add((invokable, block, overriding));
                     else
                        throw needsOverride(functionName);

                  statements.Add(statement);
               }
                  break;
               default:
                  statements.Add(statement);
                  break;
            }

         statements.Add(new ReturnNewObject(className, parameters));

         Statements = statements.ToArray();

         return new Block(statements);
      }

      public IMatched<Unit> Constructor(Parameters parameters, Block block, bool standard)
      {
         var invokable = new ConstructorInvokable(className, parameters);
         var fullFunctionName = parameters.FullFunctionName(className);
         if (constructorInvokables.ContainsKey(fullFunctionName))
            return $"Constructor {fullFunctionName} already exists".FailedMatch<Unit>();
         else
         {
            constructorInvokables[fullFunctionName] = (invokable, modifyBlock(block, standard));
            return Unit.Matched();
         }
      }

      public void Generate(OperationsBuilder builder, int index)
      {
         foreach (var item in constructorInvokables)
         {
            var functionName = item.Key;
            var (invokable, block) = item.Value;
            if (builder.RegisterInvokable(invokable, block, true).IfNot(out var exception))
               throw exception;

            builder.NewField(functionName, false, true);
            builder.PushObject(new Constructor(invokable));
            builder.Peek(index);
            builder.AssignField(functionName, true);
         }

         foreach (var function in functions)
         {
            var (invokable, block, overriding) = function;
            if (builder.RegisterInvokable(invokable, block, overriding).IfNot(out var exception))
               throw exception;
         }
      }

      public override string ToString()
      {
         return $"class {className}({parameters}){parentClassName.Extend(" of ", $"({parentArguments.Listify()})")}";
      }
   }
}