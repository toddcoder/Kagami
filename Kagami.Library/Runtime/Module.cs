using Kagami.Library.Classes;
using Kagami.Library.Packages;
using Core.Collections;
using Core.Monads;
using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Runtime;

public class Module
{
   public static Module Global { get; set; }

   protected Hash<string, BaseClass> classes = [];
   protected Hash<string, Mixin> mixins = [];
   protected Set<string> forwardReferences = [];
   protected Hash<string, string> dataReferences = [];
   protected Set<string> operators = [];

   public void LoadBuiltinClasses()
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
   }

   public Maybe<BaseClass> Class(string name, bool forwardsIncluded = false)
   {
      if (classes.ContainsKey(name))
      {
         return classes[name].Some();
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
      if (classes.ContainsKey(className))
      {
         classes[alias] = classes[className];
         return unit;
      }
      else
      {
         return classNotFound(className);
      }
   }
}