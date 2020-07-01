using System;
using System.Linq;
using Kagami.Library.Objects;
using Core.Collections;
using Core.Monads;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using Boolean = Kagami.Library.Objects.Boolean;
using String = Kagami.Library.Objects.String;

namespace Kagami.Library.Packages
{
   public class XRandom : IObject, ICollection
   {
      Random random;

      public XRandom(int seed) => random = new Random(seed);

      public XRandom() => random = new Random();

      public Float NextFloat() => random.NextDouble();

      public Int Next() => random.Next();

      public IIterator GetIterator(bool lazy) => lazy ? new Iterator(this) : new LazyIterator(this);

      IMaybe<IObject> ICollection.Next(int index) => Int.IntObject(random.Next()).Some();

      public IMaybe<IObject> Peek(int index) => Int.IntObject(random.Next()).Some();

      public Int Length => -1;

      public bool ExpandForArray => false;

      public Boolean In(IObject item) => false;

      public Boolean NotIn(IObject item) => true;

      public IObject Times(int count)
      {
         return new Objects.Tuple(Enumerable.Range(0, count).Select(i => Int.IntObject(random.Next())).ToArray());
      }

      public String MakeString(string connector) => makeString(this, connector);

      public IIterator GetIndexedIterator() => new IndexedIterator(this);

      public Int Next(int max) => random.Next(max);

      public Int Next(int min, int max) => random.Next(min, max);

      public string ClassName => "Random";

      public string AsString => "Random";

      public string Image => "Random";

      public int Hash => random.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is XRandom;

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => random.Next() != 0;

      public IObject this[SkipTake skipTake] => skipTakeThis(this, skipTake);
   }
}