using Kagami.Library.Classes;
using Kagami.Library.Packages;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Runtime
{
   public class Module
   {
      public static Module Global { get; set; }

      Hash<string, BaseClass> classes;
      Hash<string, TraitClass> traits;
      Set<string> forwardReferences;
      Hash<string, string> dataReferences;
      Set<string> operators;

      public Module()
      {
         classes = new Hash<string, BaseClass>();
         traits = new Hash<string, TraitClass>();
         forwardReferences = new Set<string>();
         dataReferences = new Hash<string, string>();
         operators = new Set<string>();
      }

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
         classes["Lambda"] = new LambaClass();
         classes["Void"] = new VoidClass();
         classes["Some"] = new SomeClass();
         classes["Nil"] = new NilClass();
         classes["Array"] = new ArrayClass();
         classes["Iterator"] = new IteratorClass();
         classes["LazyIterator"] = new LazyIteratorClass();
         classes["StreamIterator"] = new StreamIteratorClass();
         classes["Any"] = new AnyClass();
         classes["Placeholder"] = new PlaceholderClass();
         classes["Range"] = new RangeClass();
         classes["Dictionary"] = new DictionaryClass();
         classes["InternalList"] = new InternalListClass();
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
      }

      public IMaybe<BaseClass> Class(string name) => classes.Map(name);

      public IMaybe<TraitClass> Trait(string name) => traits.Map(name);

      public IResult<Unit> RegisterClass(BaseClass cls)
      {
         if (classes.ContainsKey(cls.Name))
            return failure<Unit>(classAlreadyExists(cls.Name));
         else
         {
            classes[cls.Name] = cls;
            return Unit.Success();
         }
      }

      public IResult<Unit> RegisterTrait(TraitClass trait)
      {
         if (traits.ContainsKey(trait.Name))
            return failure<Unit>(traitAlreadyExists(trait.Name));
         else
         {
            traits[trait.Name] = trait;
            return Unit.Success();
         }
      }

      public void ForwardReference(string name) => forwardReferences.Add(name);

      public bool Forwarded(string name) => forwardReferences.Contains(name);

      public void RegisterDataComparisand(string dataType, string dataComparisand) => dataReferences[dataComparisand] = dataType;

      public IMaybe<string> FullDataComparisandName(string name) => dataReferences.Map(name).Map(s => $"{s}.{name}");

      public bool RegisterOperator(string name)
      {
         if (operators.Contains(name))
            return false;
         else
         {
            operators.Add(name);
            return true;
         }
      }

      public bool OperatorExists(string name) => operators.Contains(name);

      public IResult<Unit> Alias(string alias, string className)
      {
         if (classes.ContainsKey(className))
         {
            classes[alias] = classes[className];
            return Unit.Success();
         }
         else
            return failure<Unit>(classNotFound(className));
      }
   }
}