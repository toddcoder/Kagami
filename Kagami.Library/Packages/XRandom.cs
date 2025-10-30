using Kagami.Library.Objects;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Packages;

public class XRandom : IObject, ICollection
{
   protected int seed;
   protected Random random;

   public XRandom(int seed)
   {
      this.seed = seed;

      random = new Random(seed);
   }

   public XRandom() => random = new Random();

   public Float NextFloat() => random.NextDouble();

   public Int Next() => random.Next();

   public IIterator GetIterator(bool lazy) => lazy ? new Iterator(this) : new LazyIterator(this);

   Maybe<IObject> ICollection.Next(int index)
   {
      return IterateFloats.Value ? Float.FloatObject(random.NextDouble()).Some() : Int.IntObject(random.Next()).Some();
   }

   public Maybe<IObject> Peek(int index)
   {
      return IterateFloats.Value ? Float.FloatObject(random.NextDouble()).Some() : Int.IntObject(random.Next()).Some();
   }

   public Int Length => -1;

   public bool ExpandForArray => false;

   public KBoolean In(IObject item) => false;

   public KBoolean NotIn(IObject item) => true;

   public IObject Times(int count)
   {
      return new KTuple(Enumerable.Range(0, count).Select(_ => Int.IntObject(random.Next())).ToArray());
   }

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One() => Next();

   public IObject Copy() => new XRandom(seed);

   public IIterator Following(IObject following) => new MultiIterator(this, following);

   public Maybe<TypeConstraint> TypeConstraint => nil;

   public Int Next(int max) => random.Next(max);

   public Int Next(int min, int max) => random.Next(min, max);

   public Int Next(int min, int max, int increment)
   {
      if (increment <= 0)
      {
         throw fail("Increment must be > 0");
      }

      var possibleSteps = (max - min) / increment;
      var randomStep = random.Next(0, possibleSteps + 1);

      return min + randomStep * increment;
   }

   public string ClassName => "Random";

   public string AsString => "Random";

   public string Image => "Random";

   public int Hash => random.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is XRandom;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => random.Next() != 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject this[SkipTake skipTake] => Objects.CollectionFunctions.skipTake(this, skipTake);

   public KBoolean IterateFloats { get; set; }
}