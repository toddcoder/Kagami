using Core.Collections;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Operations;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using Class = Kagami.Library.Nodes.Statements.Class;
using Return = Kagami.Library.Nodes.Statements.Return;

namespace Kagami.Library.Parsers.Statements;

public class TypeCreator(string typeName, TypeMemberData[] typeMemberData, Block commonBlock) : Statement
{
   protected Class typeClass = null!;
   protected MetaClass metaClass = null!;
   protected Class[] typeMemberClasses = [];
   protected MetaClass[] typeMetaClasses = [];

   public Optional<Unit> Create()
   {
      var builder = new ClassBuilder(typeName, Parameters.Empty, "", [], false, new Block());
      var _registered = builder.Register();
      if (!_registered)
      {
         return _registered.Exception;
      }

      typeClass = new Class(builder);

      List<Statement> statements = [];
      List<ClassBuilder> builders = [];
      List<(string, ClassBuilder)> metaBuilders = [];
      List<string> valuesList = [];
      Hash<IObject, IObject> ordinals = [];

      foreach (var data in typeMemberData)
      {
         statements.Add(getMemberFunction(data));
         var _classBuilder = getMemberClassBuilder(data, typeName, commonBlock, ordinals);
         if (_classBuilder is (true, var classBuilder))
         {
            builders.Add(classBuilder);
         }
         else
         {
            var (forClass, forMetaClass) = getMemberMetaClassBuilder(data, typeName, commonBlock, ordinals);
            builders.Add(forClass);
            metaBuilders.Add((data.Name, forMetaClass));
            valuesList.Add(forClass.ClassName);
         }
      }

      if (valuesList.Count > 0)
      {
         var expressionBuilder = new ExpressionBuilder(ExpressionFlags.Standard);
         var firstValue = valuesList[0];
         expressionBuilder.Add(new ClassSymbol(firstValue));
         foreach (var className in valuesList.Skip(1))
         {
            expressionBuilder.Add(new CommaSymbol());
            expressionBuilder.Add(new ClassSymbol(className));
         }

         var _expression = expressionBuilder.ToExpression();
         if (_expression is (true, var expression))
         {
            var setSymbol = new DictionaryOrSetSymbol(expression, nil);
            var returnBlock = new Block(new Return(new Expression(setSymbol), nil));
            var function = new Function("__$members", Parameters.Empty, false, returnBlock, false, false, "");
            statements.Add(function);
         }

         if (ordinals.Count > 0)
         {
            var dictionary = new Dictionary(ordinals);
            var pushObjectSymbol = new PushObjectSymbol(dictionary);
            var returnBlock = new Block(new Return(new Expression(pushObjectSymbol), nil));
            var function = new Function("__$values", Parameters.Empty, false, returnBlock, false, false, "");
            statements.Add(function);
         }
      }

      var staticBlock = new Block(statements);
      var metaClassName = $"__$meta{typeName}";
      var metaClassBuilder = new ClassBuilder(metaClassName, Parameters.Empty, "", [], false, staticBlock);
      _registered = metaClassBuilder.Register();
      if (!_registered)
      {
         return _registered.Exception;
      }

      metaClass = new MetaClass(typeName, metaClassBuilder);

      List<Class> classes = [];
      List<MetaClass> metaClasses = [];
      foreach (var classBuilder in builders)
      {
         _registered = classBuilder.Register();
         if (!_registered)
         {
            return _registered.Exception;
         }

         classes.Add(new Class(classBuilder));
      }

      foreach (var (className, classBuilder) in metaBuilders)
      {
         _registered = classBuilder.Register();
         if (!_registered)
         {
            return _registered.Exception;
         }

         metaClasses.Add(new MetaClass(className, classBuilder));
      }

      typeMemberClasses = [.. classes];
      typeMetaClasses = [.. metaClasses];

      return unit;
   }

   protected static Function getMemberFunction(TypeMemberData data)
   {
      Expression[] arguments = [.. data.Parameters.Select(p => new Expression(new FieldSymbol(p.Name)))];
      var invokeSymbol = new InvokeSymbol(data.Name, arguments, nil, true);
      var block = new Block(new Return(new Expression(invokeSymbol), nil));

      var functionName = arguments.Length > 0 ? data.Name.ToLower1() : data.Name.ToLower1().get();
      return new Function(functionName, data.Parameters, false, block, false, false, "");
   }

   protected static AssignToNewField getOrdinalFunction(IObject ordinal)
   {
      return new AssignToNewField(false, "value", new Expression(new PushObjectSymbol(ordinal)), false);
   }

   protected static AssignToNewField getClassField(string className)
   {
      return new AssignToNewField(false, "class", new Expression(new ClassSymbol(className)), false);
   }

   protected static Maybe<ClassBuilder> getMemberClassBuilder(TypeMemberData data, string enumClassName, Block commonBlock,
      Hash<IObject, IObject> ordinals)
   {
      var localCommonBlock = commonBlock.Clone();
      if (data.Parameters.Length > 0)
      {
         if (data.Ordinal is (true, var ordinal))
         {
            ordinals[ordinal] = new Objects.Class(data.Name);
            localCommonBlock.Add(getOrdinalFunction(ordinal));
         }
         else
         {
            var value = KString.StringObject(data.Name.ToLower1());
            ordinals[value] = new Objects.Class(data.Name);
            localCommonBlock.Add(getOrdinalFunction(value));
         }

         return new ClassBuilder(data.Name, data.Parameters, enumClassName, [], false, localCommonBlock);
      }
      else
      {
         return nil;
      }
   }

   protected static (ClassBuilder, ClassBuilder) getMemberMetaClassBuilder(TypeMemberData data, string enumClassName, Block commonBlock,
      Hash<IObject, IObject> ordinals)
   {
      var localCommonBlock = commonBlock.Clone();
      var classBuilder = new ClassBuilder(data.Name, Parameters.Empty, enumClassName, [], false, new Block());
      if (data.Parameters.Length == 0)
      {
         if (data.Ordinal is (true, var ordinal))
         {
            ordinals[ordinal] = new Objects.Class(data.Name);
            localCommonBlock.Add(getOrdinalFunction(ordinal));
         }
         else
         {
            var value = KString.StringObject(data.Name.ToLower1());
            localCommonBlock.Add(getOrdinalFunction(value));
         }
      }

      localCommonBlock.Add(getClassField(data.Name));

      var metaClassBuilder = new ClassBuilder($"__meta{data.Name}", Parameters.Empty, "", [], false, localCommonBlock);
      return (classBuilder, metaClassBuilder);
   }

   public override void Generate(OperationsBuilder builder)
   {
      typeClass.Generate(builder);
      metaClass.Generate(builder);
      foreach (var enumMemberClass in typeMemberClasses)
      {
         enumMemberClass.Generate(builder);
      }

      foreach (var enumMetaClass in typeMetaClasses)
      {
         enumMetaClass.Generate(builder);
      }
   }
}