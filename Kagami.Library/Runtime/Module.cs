using Kagami.Library.Classes;
using Kagami.Library.Packages;
using Core.Collections;
using Core.Monads;
using Core.Objects;
using Kagami.Library.Objects;
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
      "Container" => new ContainerClass(),
      "Unmatched" => new UnmatchedClass(),
      "Complex" => new ComplexClass(),
      "Rational" => new RationalClass(),
      "Long" => new LongClass(),
      "Lazy" => new LazyClass(),
      "YieldingInvokable" => new YieldingInvokableClass(),
      "Del" => new DelClass(),
      "Slice" => new SliceClass(),
      "End" => new EndClass(),
      "List" => new ListClass(),
      "Arguments" => new ArgumentsClass(),
      "Symbol" => new SymbolClass(),
      "Infinity" => new InfinityClass(),
      "OpenRange" => new OpenRangeClass(),
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
      _ => nil
   };

   protected LazyMemo<string, BaseClass> classes = new(getBuiltinClass);
   protected Hash<string, Mixin> mixins = [];
   protected Set<string> forwardReferences = [];
   protected Hash<string, string> dataReferences = [];
   protected Set<string> operators = [];

   /*public void LoadBuiltinClasses()
   {
      classes["Int"] = new IntClass();
      classes["Float"] = new FloatClass();
      classes["Boolean"] = new BooleanClass();
      classes["String"] = new StringClass();
      classes["Char"] = new CharClass();
      classes["Byte"] = new ByteClass();
      classes["Message"] = new MessageClass();
      classes["Unassigned"] = new UnassignedClass();
      classes["Tuple"] = new TupleClass();
      classes["NameValue"] = new NameValueClass();
      classes["Lambda"] = new LambdaClass();
      classes["Void"] = new VoidClass();
      classes["Some"] = new SomeClass();
      classes["None"] = new NoneClass();
      classes["Array"] = new ArrayClass();
      classes["Iterator"] = new IteratorClass();
      classes["LazyIterator"] = new LazyIteratorClass();
      classes["StreamIterator"] = new StreamIteratorClass();
      classes["Any"] = new AnyClass();
      classes["Placeholder"] = new PlaceholderClass();
      classes["Range"] = new RangeClass();
      classes["Dictionary"] = new DictionaryClass();
      classes["Container"] = new ContainerClass();
      classes["Unmatched"] = new UnmatchedClass();
      classes["Complex"] = new ComplexClass();
      classes["Rational"] = new RationalClass();
      classes["Long"] = new LongClass();
      classes["Lazy"] = new LazyClass();
      classes["YieldingInvokable"] = new YieldingInvokableClass();
      classes["Del"] = new DelClass();
      classes["Slice"] = new SliceClass();
      classes["End"] = new EndClass();
      classes["List"] = new ListClass();
      classes["Arguments"] = new ArgumentsClass();
      classes["Symbol"] = new SymbolClass();
      classes["Infinity"] = new InfinityClass();
      classes["OpenRange"] = new OpenRangeClass();
      classes["KeyValue"] = new KeyValueClass();
      classes["Regex"] = new RegexClass();
      classes["Pattern"] = new PatternClass();
      classes["PackageFunction"] = new PackageFunctionClass();
      classes["Sys"] = new SysClass();
      classes["Math"] = new MathClass();
      classes["RuntimeFunction"] = new RuntimeFunctionClass();
      classes["Reference"] = new ReferenceClass();
      classes["Group"] = new RegexGroupClass();
      classes["Match"] = new RegexMatchClass();
      classes["Date"] = new DateClass();
      classes["Interval"] = new IntervalClass();
      classes["TypeConstraint"] = new TypeConstraintClass();
      classes["ByteArray"] = new ByteArrayClass();
      classes["Selector"] = new SelectorClass();
      classes["Number"] = new NumberClass();
      classes["Collection"] = new CollectionClass();
      classes["TextFinding"] = new TextFindingClass();
      classes["SkipTake"] = new SkipTakeClass();
      classes["Constructor"] = new ConstructorClass();
      classes["MutString"] = new MutStringClass();
      classes["Error"] = new ErrorClass();
      classes["Success"] = new SuccessClass();
      classes["Failure"] = new FailureClass();
      classes["Optional"] = new OptionalClass();
      classes["Result"] = new ResultClass();
      classes["Monad"] = new MonadClass();
      classes["Unit"] = new UnitClass();
      classes["YieldReturn"] = new YieldReturnClass();
      classes["Index"] = new IndexClass();
      classes["Cycle"] = new CycleClass();
      classes["Set"] = new SetClass();
   }*/

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

   public Maybe<Mixin> Mixin(string name) => mixins.Maybe[name];

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

   public void RegisterMixin(Mixin mixin)
   {
      if (!mixins.ContainsKey(mixin.ClassName))
      {
         mixins[mixin.ClassName] = mixin;
      }
   }

   public void ForwardReference(string name) => forwardReferences.Add(name);

   public bool Forwarded(string name) => forwardReferences.Contains(name);

   public void RegisterDataComparisand(string dataType, string dataComparisand) => dataReferences[dataComparisand] = dataType;

   public Maybe<string> FullDataComparisandName(string name) => dataReferences.Maybe[name].Map(s => $"{s}.{name}");

   public bool RegisterOperator(string name)
   {
      if (operators.Contains(name))
      {
         return false;
      }
      else
      {
         operators.Add(name);
         return true;
      }
   }

   public bool OperatorExists(string name) => operators.Contains(name);

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
}