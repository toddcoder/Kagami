using Kagami.Library.Classes;
using Kagami.Library.Packages;
using Core.Collections;
using Core.Monads;
using Core.Objects;
using Kagami.Library.Inclusions;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Runtime;

public class Module
{
   public static LateLazy<Module> Global { get; set; } = new(true);

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
      "None" => new NoneClass(),
      "Array" => new ArrayClass(),
      "Iterator" => new IteratorClass(),
      "LazyIterator" => new LazyIteratorClass(),
      "StreamIterator" => new StreamIteratorClass(),
      "Any" => new AnyClass(),
      "Placeholder" => new PlaceholderClass(),
      "Range" => new RangeClass(),
      "Dictionary" => new DictionaryClass(),
      "Sequence" => new SequenceClass(),
      "Unmatched" => new UnmatchedClass(),
      "Complex" => new ComplexClass(),
      "Rational" => new RationalClass(),
      "Long" => new LongClass(),
      "Lazy" => new LazyClass(),
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
      "Index" => new IndexClass(),
      "Cycle" => new CycleClass(),
      "Set" => new SetClass(),
      "Decimal" => new DecimalClass(),
      "PendingRegex" => new PendingRegexClass(),
      "UserObjectPlaceholder" => new UserObjectPlaceholderClass(),
      _ => nil
   };

   protected LazyMemo<string, BaseClass> classes = new(getBuiltinClass);
   protected StringHash<Inclusion> inclusions = [];
   protected Set<string> forwardReferences = [];
   protected Hash<string, string> dataReferences = [];
   protected StringHash<OperatorType> operators = [];
   protected Hash<Guid, string> bindings = [];
   protected Hash<Guid, string> retrievedFields = [];

   public Maybe<BaseClass> Class(string name, bool forwardsIncluded = false)
   {
      if (classes.Maybe[name] is (true, var @class))
      {
         return @class;
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
}