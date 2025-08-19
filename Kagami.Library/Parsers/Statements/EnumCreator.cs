using Core.Monads;
using Core.Strings;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using Return = Kagami.Library.Nodes.Statements.Return;

namespace Kagami.Library.Parsers.Statements;

public class EnumCreator(string enumName, EnumMemberData[] enumMemberData, Block commonBlock) : Statement
{
   protected Class enumClass = null!;
   protected MetaClass metaClass = null!;
   protected Class[] enumMemberClasses = [];
   protected MetaClass[] enumMetaClasses = [];

   public Optional<Unit> Create()
   {
      var builder = new ClassBuilder(enumName, Parameters.Empty, "", [], false, new Block());
      var _registered = builder.Register();
      if (!_registered)
      {
         return _registered.Exception;
      }

      enumClass = new Class(builder);

      List<Statement> statements = [];
      List<ClassBuilder> builders = [];
      List<(string, ClassBuilder)> metaBuilders = [];
      foreach (var data in enumMemberData)
      {
         statements.Add(getMemberFunction(data));
         var _classBuilder = getMemberClassBuilder(data, enumName, commonBlock);
         if (_classBuilder is (true, var classBuilder))
         {
            builders.Add(classBuilder);
         }
         else
         {
            var (forClass, forMetaClass) = getMemberMetaClassBuilder(data, enumName, commonBlock);
            builders.Add(forClass);
            metaBuilders.Add((data.Name, forMetaClass));
         }
      }

      var staticBlock = new Block(statements);

      var metaClassName = $"__$meta{enumName}";
      var metaClassBuilder = new ClassBuilder(metaClassName, Parameters.Empty, "", [], false, staticBlock);
      _registered = metaClassBuilder.Register();
      if (!_registered)
      {
         return _registered.Exception;
      }

      metaClass = new MetaClass(enumName, metaClassBuilder);

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

      enumMemberClasses = [.. classes];
      enumMetaClasses = [.. metaClasses];

      return unit;
   }

   protected static Function getMemberFunction(EnumMemberData data)
   {
      Expression[] arguments = [.. data.Parameters.Select(p => new Expression(new FieldSymbol(p.Name)))];
      var invokeSymbol = new InvokeSymbol(data.Name, arguments, nil, true);
      var block = new Block(new Return(new Expression(invokeSymbol), nil));

      var functionName = arguments.Length > 0 ? data.Name.ToLower1() : data.Name.ToLower1().get();
      return new Function(functionName, data.Parameters, block, false, false, "");
   }

   protected static Maybe<ClassBuilder> getMemberClassBuilder(EnumMemberData data, string enumClassName, Block commonBlock)
   {
      if (data.Parameters.Length > 0)
      {
         return new ClassBuilder(data.Name, data.Parameters, enumClassName, [], false, commonBlock);
      }
      else
      {
         return nil;
      }
   }

   protected static (ClassBuilder, ClassBuilder) getMemberMetaClassBuilder(EnumMemberData data, string enumClassName, Block commonBlock)
   {
      var classBuilder = new ClassBuilder(data.Name, Parameters.Empty, enumClassName, [], false, new Block());
      var metaClassBuilder = new ClassBuilder($"__meta{data.Name}", Parameters.Empty, "", [], false, commonBlock);
      return (classBuilder, metaClassBuilder);
   }

   public override void Generate(OperationsBuilder builder)
   {
      enumClass.Generate(builder);
      metaClass.Generate(builder);
      foreach (var enumMemberClass in enumMemberClasses)
      {
         enumMemberClass.Generate(builder);
      }

      foreach (var enumMetaClass in enumMetaClasses)
      {
         enumMetaClass.Generate(builder);
      }
   }
}