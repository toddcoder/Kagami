using Core.Collections;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Objects
{
   public readonly struct OpenRange : IObject, ICollection
   {
      private readonly IObject seed;
      private readonly Lambda lambda;

      public OpenRange(IObject seed, Lambda lambda) : this()
      {
         this.seed = seed;
         this.lambda = lambda;
      }

      public IObject Seed => seed;

      public Lambda Lambda => lambda;

      public string ClassName => "OpenRange";

      public string AsString => $"{seed.AsString} ** {lambda.AsString}";

      public string Image => $"{seed.Image} ** {lambda.Image}";

      public int Hash => (seed.Hash + lambda.Hash).GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is OpenRange range && seed.IsEqualTo(range.seed) && lambda.IsEqualTo(range.lambda);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => seed.IsTrue;

      public IIterator GetIterator(bool lazy) => new OpenRangeCollection(this).GetIterator(lazy);

      public Maybe<IObject> Next(int index) => nil;

      public Maybe<IObject> Peek(int index) => nil;

      public Int Length => -1;

      public bool ExpandForArray => false;

      public Boolean In(IObject item) => false;

      public Boolean NotIn(IObject item) => false;

      public IObject Times(int count) => this;

      public String MakeString(string connector) => makeString(this, connector);

      public IIterator GetIndexedIterator() => new IndexedIterator(this);

      public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
   }
}