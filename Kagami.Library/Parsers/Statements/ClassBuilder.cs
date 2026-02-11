using Kagami.Library.Classes;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Runtime;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.CommonFunctions;
using static Core.Monads.MonadFunctions;
using DefineNewField = Kagami.Library.Nodes.Statements.DefineNewField;
using Return = Kagami.Library.Nodes.Statements.Return;

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
   protected Set<RequireFunctionMatch> requiredFunctions = [];
   protected StringHash<Expression> delegates = [];
   protected StringHash<RequiredField> requiredFields = [];
   protected List<Statement> mixinStatements = [];

   public ClassBuilder(string className, Parameters parameters, string parentClassName, Expression[] parentArguments,
      bool initialize, Block constructorBlock, bool isFixed = false)
   {
      this.className = className;
      this.parameters = parameters;
      this.parentClassName = parentClassName;
      this.parentArguments = parentArguments;
      this.initialize = initialize;
      this.constructorBlock = constructorBlock;

      IsFixed = isFixed;
   }

   public string ClassName => className;

   public virtual UserClass CreateUserClass() => new(className, parentClassName);

   public Optional<Unit> Register()
   {
      userClass = CreateUserClass();
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

   public Parameters Parameters => parameters;

   public UserClass UserClass => userClass;

   public Statement[] Statements { get; set; } = [];

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

      if (mixinStatements.Count > 0)
      {
         Set<Selector> abstracts = [];
         Set<Selector> implemented = [];
         foreach (var statement in mixinStatements.Where(isModifiable))
         {
            switch (statement)
            {
               case Function { IsAbstract: true } abstractFunction:
               {
                  abstracts.Add(abstractFunction.Selector);
                  break;
               }
               case Function function when function.IsGetter || function.IsSetter:
               {
                  continue;
               }
               case Function function:
               {
                  implemented.Add(function.Selector);
                  originalBlock.Add(function);
                  break;
               }
               case MatchFunction matchFunction:
               {
                  implemented.Add(matchFunction.Selector);
                  originalBlock.Add(matchFunction);
                  break;
               }
               default:
                  originalBlock.Add(statement);
                  break;
            }
         }

         foreach (var statement in originalBlock)
         {
            switch (statement)
            {
               case Function { IsAbstract: false } function:
               {
                  implemented.Add(function.Selector);
                  break;
               }
               case MatchFunction matchFunction:
               {
                  implemented.Add(matchFunction.Selector);
                  break;
               }
            }
         }

         foreach (var selector in abstracts.Where(selector => !implemented.Contains(selector)))
         {
            throw fail($"{selector} is not implemented");
         }
      }

      foreach (var statement in originalBlock)
      {
         switch (statement)
         {
            case AssignToNewField { Ignore: false } assignToNewField:
            {
               var (mutable, fieldName, _typeConstraint, isHidden, isOverride, _) = assignToNewField;
               if (isHidden)
               {
                  statements.Add(statement);
               }
               else
               {
                  processField(fieldName, _typeConstraint, mutable, isOverride, statement);
               }

               break;
            }
            case AssignToNewField2 assignToNewField2:
            {
               var (comparisand, _) = assignToNewField2;
               if (comparisand.Symbols[0] is PlaceholderSymbol placeholder)
               {
                  var fieldName = placeholder.Name;
                  var (bindingType, name) = fromBindingName(fieldName);
                  var function = Function.Getter(name, false);
                  statements.Add(function);
                  var (functionName, _, block, _, invokable, _, isHidden) = function;
                  if (isHidden)
                  {
                     statements.Add(statement);
                     break;
                  }
                  else
                  {
                     if (!userClass.RegisterMethod(functionName, new Lambda(invokable, false), true))
                     {
                        throw needsOverride(functionName);
                     }
                  }

                  functions.Add((invokable, block, true));

                  if (bindingType == BindingType.Mutable)
                  {
                     function = Function.Setter(fieldName, false);
                     statements.Add(function);
                     (functionName, _, block, _, invokable, _, isHidden) = function;
                     if (isHidden)
                     {
                        statements.Add(statement);
                        break;
                     }
                     else
                     {
                        if (!userClass.RegisterMethod(functionName, new Lambda(invokable, false), true))
                        {
                           throw needsOverride(functionName);
                        }
                     }

                     functions.Add((invokable, block, true));
                  }

                  statements.Add(statement);
               }

               break;
            }
            case DefineNewField defineNewField:
            {
               var (mutable, fieldName, typeName, isHidden, isOverride, _) = defineNewField;
               var typeConstraint = TypeConstraint.FromList(typeName);
               if (isHidden)
               {
                  statements.Add(statement);
               }
               else
               {
                  processField(fieldName, typeConstraint, mutable, isOverride, statement);
               }

               break;
            }
            case CreateNewFields createNewFields:
            {
               if (createNewFields.IsHidden)
               {
                  statements.Add(statement);
               }
               else
               {
                  var typeConstraint = TypeConstraint.FromList(createNewFields.ClassName);
                  foreach (var fieldName in createNewFields.Fields)
                  {
                     processField(fieldName, typeConstraint, true, false, statement);
                  }
               }

               break;
            }
            case LazyAssign lazyAssign:
            {
               if (lazyAssign.IsHidden)
               {
                  statements.Add(statement);
               }
               else
               {
                  processField(lazyAssign.FieldName, nil, false, false, statement);
               }

               break;
            }
            case AssignDefinition assignDefinition:
            {
               var (fieldName, _) = assignDefinition;
               processField(fieldName, nil, false, false, statement);
               break;
            }
            case Function function when standard:
            {
               var (selector, _, block, _, invokable, overriding, isHidden) = function;
               var _typeConstraint = block.TypeConstraint;
               if (isHidden)
               {
                  statements.Add(statement);
                  break;
               }
               else
               {
                  if (userClass.RegisterMethod(selector, new Lambda(invokable, false), overriding))
                  {
                     functions.Add((invokable, block, overriding));
                  }
                  else
                  {
                     throw needsOverride(selector);
                  }
               }

               if (requiredFunctions.FirstOrNone(f => f.Matches(selector, _typeConstraint)) is (true, var requireFunctionMatch))
               {
                  requiredFunctions.Remove(requireFunctionMatch);
               }

               statements.Add(statement);
               break;
            }
            case MatchFunction matchFunction when standard:
            {
               var (functionName, _, block, _, invokable, overriding, isHidden) = matchFunction;
               if (!isHidden)
               {
                  if (userClass.RegisterMethod(functionName, new Lambda(invokable, false), overriding))
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
               requiredFunctions.Add(new RequireFunctionMatch(requiredFunction.Selector, requiredFunction.TypeConstraint));
               userClass.RegisterInclusion(requiredFunction.Inclusion);
               break;
            }
            case RequiredField requiredField:
               requiredFields[requiredField.FieldName] = requiredField;
               break;
            default:
               statements.Add(statement);
               break;
         }
      }

      foreach (var parameter in parameters)
      {
         if (requiredFields.Maybe[parameter.Name] is (true, var requiredField))
         {
            var _result = requiredField.Require(parameter.Name, parameter.TypeConstraint, parameter.Mutable);
            requiredFields.Maybe[parameter.Name] = _result ? nil : throw _result.Exception;
         }
      }

      if (requiredFunctions.Count > 0)
      {
         var functionList = requiredFunctions.ToString(", ");
         throw fail(requiredFunctions.Count.Plural($"Required function(s) {functionList} not implemented"));
      }

      if (requiredFields.Count > 0)
      {
         var fieldList = requiredFields.Select(rf => rf.Value.FieldName).ToString(", ");
         throw fail(requiredFields.Count.Plural($"Required field(s) {fieldList} not implemented"));
      }

      foreach (var (delegateClass, delegateConstructor) in delegates)
      {
         statements.Add(new NewDelegateStatement(className, delegateClass, delegateConstructor));
      }

      statements.Add(new ReturnNewObject(className, parameters));

      Statements = [.. statements];

      return new Block(statements);

      void processField(string fieldName, Maybe<TypeConstraint> _typeConstraint, bool mutable, bool isOverride, Statement statement)
      {
         if (requiredFields.Maybe[fieldName] is (true, var requiredField))
         {
            var _result = requiredField.Require(fieldName, _typeConstraint, mutable);
            requiredFields.Maybe[fieldName] = _result ? nil : throw _result.Exception;
         }

         var function = Function.Getter(fieldName, isOverride);
         statements.Add(function);
         var (functionName, _, block, _, invokable, _, isHidden) = function;
         if (!isHidden && !userClass.RegisterMethod(functionName, new Lambda(invokable, false), isOverride))
         {
            throw needsOverride(functionName);
         }

         functions.Add((invokable, block, isOverride));

         if (mutable)
         {
            function = Function.Setter(fieldName, isOverride);
            statements.Add(function);
            (functionName, _, block, _, invokable, _, isHidden) = function;
            if (!isHidden && !userClass.RegisterMethod(functionName, new Lambda(invokable, false), isOverride))
            {
               throw needsOverride(functionName);
            }

            functions.Add((invokable, block, isOverride));
         }

         statements.Add(statement);
      }

      bool isModifiable(Statement statement) => statement is AssignToNewField or AssignToNewField2 or DefineNewField or CreateNewFields or LazyAssign
         or AssignDefinition or Function or MatchFunction;
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

   public void Generate(OperationsBuilder builder)
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

   public void RegisterDelegate(string className, Expression constructor) => delegates[className] = constructor;

   public void AddMixin(MetaClass metaClass)
   {
      mixinStatements.AddRange(metaClass.ClassBuilder.Statements.Where(s => s is not Return));
   }

   public override string ToString()
   {
      return $"class {className}({parameters}){parentClassName.Map(s => $"{s} of ({parentArguments.ToString(", ")})")}";
   }

   public bool IsFixed { get; set; }
}