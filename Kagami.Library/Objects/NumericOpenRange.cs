using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public readonly struct NumericOpenRange(INumeric seed, INumeric increment) : IObject, ICollection
{
   private readonly IObject seedAsObject = (IObject)seed;

   private readonly IObject incrementAsObject = (IObject)increment;

   public IObject Seed => seedAsObject;

   public IObject Increment => incrementAsObject;

   public string ClassName => "NumericOpenRange";

   public string AsString => $"{seedAsObject.AsString} ** {incrementAsObject.AsString}";

   public string Image => $"{seedAsObject.AsString} ** {incrementAsObject.AsString}";

   public int Hash => HashCode.Combine(seedAsObject.Hash, incrementAsObject.Hash);

   public bool IsEqualTo(IObject obj) => obj is NumericOpenRange other && seedAsObject.IsEqualTo(other.seedAsObject) &&
      incrementAsObject.IsEqualTo(other.incrementAsObject);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => seedAsObject.IsTrue;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);

   public IIterator GetIterator(bool lazy) => new NumericOpenRangeCollection(this).GetIterator(lazy);

   public Maybe<IObject> Next(int index) => nil;

   public Maybe<IObject> Peek(int index) => nil;

   public Int Length => -1;

   public bool ExpandForArray => false;

   public KBoolean In(IObject item) => false;

   public KBoolean NotIn(IObject item) => false;

   public IObject Times(int count) => this;

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One() => this;

   public IObject Copy() => new NumericOpenRange(seed, increment);

   public IIterator Following(IObject following) => new MultiIterator(this, following);
}