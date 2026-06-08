using Kagami.Library.Classes;
using Kagami.Library.Packages;
using Core.Collections;
using Core.Monads;
using Core.Objects;
using Kagami.Library.Inclusions;
using Kagami.Library.Iterators;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Objects;
using Kagami.Library.Parsers;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.CommonFunctions;

namespace Kagami.Library.Runtime;

public class Module
{
   public static LateLazy<Module> Global { get; set; } = new(true);

   protected static Hash<(string from, string to), Func<IObject, IObject>> autoConversions = [];

   protected static Hash<string, MetaClass> mixins = [];

   static Module()
   {
      autoConversions[("Int", "Float")] = i => Float.FloatObject(((Int)i).AsDouble());
      autoConversions[("Byte", "Int")] = b => Int.IntObject(((KByte)b).AsInt32());
      autoConversions[("Byte", "Float")] = b => Float.FloatObject(((KByte)b).AsDouble());
      autoConversions[("String", "MutString")] = s => new MutString(s.AsString);
      autoConversions[("MutString", "String")] = m => new KString(m.AsString);
      autoConversions[("Word", "String")] = w => (KString)((Word)w).Text;
      autoConversions[("Int", "Long")] = i => Long.LongObject(((Int)i).AsBigInteger());
      autoConversions[("Before", "Boolean")] = _ => KBoolean.True;
      autoConversions[("Int", "Char")] = i => KChar.CharObject((char)((Int)i).AsInt32());
      autoConversions[("Char", "Int")] = c => Int.IntObject(((KChar)c).Value);
      autoConversions[("Junction", "Boolean")] = j => (KBoolean)j.IsTrue;
      autoConversions[("Int", "Rational")] = i =>
      {
         var (numerator, denominator) = ((Int)i).AsRational();
         return new Rational(numerator, denominator);
      };
      autoConversions[("Float", "Rational")] = f =>
      {
         var (numerator, denominator) = ((Float)f).AsRational();
         return new Rational(numerator, denominator);
      };
      autoConversions[("Long", "Rational")] = l =>
      {
         var (numerator, denominator) = ((Long)l).AsRational();
         return new Rational(numerator, denominator);
      };
      autoConversions[("Tuple", "Complex")] = t => tupleToComplex(t);
   }

   public static Maybe<Func<IObject, IObject>> AutoConversion(string from, string to) => autoConversions.Maybe[(from, to)];

   public static Result<Unit> RegisterAutoConversion(string from, string to, Lambda lambda)
   {
      var key = (from, to);
      autoConversions[key] = from => lambda.Invoke(from);
      return unit;
   }

   protected static Maybe<BaseClass> getBuiltinClass(string name) => name switch
   {
      "Int" => new IntClass(),
      "Float" => new FloatClass(),
      "Boolean" => new BooleanClass(),
      "String" => new StringClass(),
      "Char" => new CharClass(),
      "Byte" => new ByteClass(),
      "Message" => new MessageClass(),
      "Unassigned" => new UnassignedClass(),
      "Tuple" => new TupleClass(),
      "NameValue" => new NameValueClass(),
      "Lambda" => new LambdaClass(),
      "Void" => new VoidClass(),
      "Some" => new SomeClass(),
      "Nil" => new NilClass(),
      "Array" => new ArrayClass(),
      "Iterator" => new IteratorClass(),
      "LazyIterator" => new LazyIteratorClass(),
      "Any" => new AnyClass(),
      "Placeholder" => new PlaceholderClass(),
      "Range" => new RangeClass(),
      "Dictionary" => new DictionaryClass(),
      "Sequence" => new SequenceClass(),
      "Unmatched" => new UnmatchedClass(),
      "Complex" => new ComplexClass(),
      "Rational" => new RationalClass(),
      "Long" => new LongClass(),
      "YieldingInvokable" => new YieldingInvokableClass(),
      "Slice" => new SliceClass(),
      "End" => new EndClass(),
      "List" => new ListClass(),
      "Arguments" => new ArgumentsClass(),
      "Symbol" => new SymbolClass(),
      "Infinity" => new InfinityClass(),
      "OpenRange" => new OpenRangeClass(),
      "NumericOpenRange" => new NumericOpenRangeClass(),
      "KeyValue" => new KeyValueClass(),
      "Regex" => new RegexClass(),
      "Pattern" => new PatternClass(),
      "PackageFunction" => new PackageFunctionClass(),
      "Sys" => new SysClass(),
      "Math" => new MathClass(),
      "RuntimeFunction" => new RuntimeFunctionClass(),
      "Reference" => new ReferenceClass(),
      "Group" => new RegexGroupClass(),
      "Match" => new RegexMatchClass(),
      "Date" => new DateClass(),
      "Interval" => new IntervalClass(),
      "TypeConstraint" => new TypeConstraintClass(),
      "ByteArray" => new ByteArrayClass(),
      "Selector" => new SelectorClass(),
      "Number" => new NumberClass(),
      "Collection" => new CollectionClass(),
      "TextFinding" => new TextFindingClass(),
      "SkipTake" => new SkipTakeClass(),
      "Constructor" => new ConstructorClass(),
      "MutString" => new MutStringClass(),
      "Error" => new ErrorClass(),
      "Success" => new SuccessClass(),
      "Failure" => new FailureClass(),
      "Optional" => new OptionalClass(),
      "Result" => new ResultClass(),
      "Monad" => new MonadClass(),
      "Unit" => new UnitClass(),
      "YieldReturn" => new YieldReturnClass(),
      "Cycle" => new CycleClass(),
      "Set" => new SetClass(),
      "Decimal" => new DecimalClass(),
      "PendingRegex" => new PendingRegexClass(),
      "Undefined" => new UndefinedClass(),
      "Word" => new WordClass(),
      "Words" => new WordsClass(),
      "Before" => new BeforeClass(),
      "FloatRange" => new FloatRangeClass(),
      "LazyString" => new LazyStringClass(),
      "StreamingIterator" => new StreamingIteratorClass(),
      "SpecialComparisand" => new SpecialComparisandClass(),
      "Formatter" => new FormatterClass(),
      "LongRange" => new LongRangeClass(),
      "Junction" => new JunctionClass(),
      "Event" => new EventClass(),
      "Cons" => new ConsClass(),
      "Singleton" => new SingletonClass(),
      "Definition" => new DefinitionClass(),
      "DateIncrement" => new DateIncrementClass(),
      "ProtocolWrapper" => new ProtocolWrapperClass(),
      _ => nil
   };

   public static ICollectionClass CollectionClass(ICollection collection)
   {
      if (Global.Value.Class(((IObject)collection).ClassName) is (true, var baseClass))
      {
         return baseClass as ICollectionClass ?? new ArrayClass();
      }
      else
      {
         return new ArrayClass();
      }
   }

   public static bool IsBuiltInClass(string name) => getBuiltinClass(name);

   protected Hash<string, BaseClass> classes = [];
   protected StringHash<Inclusion> inclusions = [];
   protected Set<string> forwardReferences = [];
   protected Hash<string, string> dataReferences = [];
   protected StringHash<OperatorType> operators = [];
   protected Hash<Guid, string> bindings = [];
   protected Hash<Guid, string> retrievedFields = [];
   protected Hash<(string from, string to), Selector> conversionFunctions = [];

   public Maybe<BaseClass> Class(string name, bool forwardsIncluded = false)
   {
      if (classes.Maybe[name] is (true, var @class))
      {
         return @class;
      }
      else
      {
         var _builtInClass = getBuiltinClass(name);
         if (_builtInClass is (true, var builtInClass))
         {
            classes[name] = builtInClass;
            return builtInClass;
         }
         else if (forwardsIncluded)
         {
            return new ForwardedClass(name);
         }
         else
         {
            return nil;
         }
      }
   }

   public Maybe<Inclusion> Inclusion(string name) => inclusions.Maybe[name];

   public Result<Unit> RegisterClass(BaseClass cls)
   {
      // ReSharper disable once CanSimplifyDictionaryLookupWithTryAdd
      if (classes.ContainsKey(cls.Name))
      {
         return classAlreadyExists(cls.Name);
      }
      else
      {
         classes[cls.Name] = cls;
         return unit;
      }
   }

   public void RegisterInclusion(Inclusion inclusion)
   {
      if (!inclusions.ContainsKey(inclusion.Name))
      {
         inclusions[inclusion.Name] = inclusion;
      }
   }

   public void ForwardReference(string name) => forwardReferences.Add(name);

   public bool Forwarded(string name) => forwardReferences.Contains(name);

   public void RegisterDataComparisand(string dataType, string dataComparisand) => dataReferences[dataComparisand] = dataType;

   public Maybe<string> FullDataComparisandName(string name) => dataReferences.Maybe[name].Map(s => $"{s}.{name}");

   public bool RegisterOperator(OperatorType operatorType)
   {
      if (operators.ContainsKey(operatorType.FunctionName))
      {
         return false;
      }
      else
      {
         operators[operatorType.FunctionName] = operatorType;
         return true;
      }
   }

   public Maybe<OperatorType> GetOperator(string functionName, Arity arity)
   {
      var _operator = operators.Maybe[functionName];
      if (_operator is (true, var operatorType))
      {
         if (arity is Arity.Binary && operatorType is OperatorType.Infix || arity is Arity.Prefix && operatorType is OperatorType.Prefix ||
             arity is Arity.Postfix && operatorType is OperatorType.Postfix)
         {
            return operatorType;
         }
      }

      return nil;
   }

   public Result<Unit> Alias(string alias, string className)
   {
      if (classes.Maybe[className] is (true, var @class))
      {
         classes[alias] = @class;
         return unit;
      }
      else
      {
         return classNotFound(className);
      }
   }

   public Hash<Guid, string> Bindings => bindings;

   public Hash<Guid, string> RetrievedFields => retrievedFields;

   public void RegisterConversion(string fromClass, string toClass, Selector selector) => conversionFunctions[(fromClass, toClass)] = selector;

   public Maybe<Selector> GetConversion(string fromClass, string toClass) => conversionFunctions.Maybe[(fromClass, toClass)];

   public static void RegisterMixin(string name, MetaClass mixin) => mixins[name] = mixin;

   public static Maybe<MetaClass> GetMixin(string name) => mixins.Maybe[name];
}