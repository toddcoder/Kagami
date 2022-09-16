using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
   public class Set : IObject, ICollection, IObjectCompare
   {
      public static IObject Empty => new Set();

      protected Set<IObject> set;
      protected List<IObject> list;

      public Set(IObject[] items)
      {
         set = new Set<IObject>(items);
         list = new List<IObject>();
      }

      public Set(Set<IObject> set)
      {
         this.set = set;
         list = new List<IObject>();
      }

      public Set(Set otherSet)
      {
         set = otherSet.set;
         list = new List<IObject>();
      }

      public Set(IObject obj)
      {
         set = new Set<IObject>(obj);
         list = new List<IObject>();
      }

      public Set()
      {
         set = new Set<IObject>();
         list = new List<IObject>();
      }

      public string ClassName => "Set";

      public string AsString => set.Select(i => i.AsString).ToString(" ");

      public string Image => $"⎩{set.Select(i => i.Image).ToString(", ")}⎭";

      public int Hash => set.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Set s && set.Equals(s.set);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => set.Count > 0;

      public IIterator GetIterator(bool lazy)
      {
         list = new List<IObject>();
         list.AddRange(set);

         return lazy ? new LazyIterator(this) : new Iterator(this);
      }

      public IMaybe<IObject> Next(int index) => maybe(index < set.Count, () => list[index]);

      public IMaybe<IObject> Peek(int index) => Next(index);

      public Int Length => set.Count;

      public bool ExpandForArray => true;

      public Boolean In(IObject item) => set.Contains(item);

      public Boolean NotIn(IObject item) => !set.Contains(item);

      public IObject Times(int count) => this;

      public String MakeString(string connector) => makeString(this, connector);

      public IIterator GetIndexedIterator() => new IndexedIterator(this);

      public Set Append(IObject item)
      {
         set.Add(item);
         return this;
      }

      public Set Remove(IObject item)
      {
         set.Remove(item);
         return this;
      }

      public Set Union(Set other) => new(set.Union(other.set));

      public Set Difference(Set other) => new(set.Except(other.set).ToArray());

      public Set Intersection(Set other) => new(set.Intersection(other.set));

      public IObject this[int index]
      {
         get
         {
            list = new List<IObject>();
            list.AddRange(set);
            return list[wrapIndex(index, set.Count)];
         }
      }

      public IObject Extend()
      {
         var iterator = GetIterator(false);
         var _next = iterator.Next();
         while (_next.If(out var next))
         {
            Append(next);
            _next = iterator.Next();
         }

         return this;
      }

      public IObject Clear()
      {
         set.Clear();
         return this;
      }

      public int Compare(IObject obj)
      {
         return obj switch
         {
            Set otherSet when set.IsProperSubsetOf(otherSet.set) => -1,
            Set otherSet when set.IsSubsetOf(otherSet.set) => 0,
            Set => 1,
            _ => throw AllExceptions.unableToConvert(obj.Image, "Set")
         };
      }

      public IObject Object => this;

      public Boolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

      public Boolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

      public Set XOr(Set other) => Union(other).Difference(Intersection(other));

      public Set Classify(Lambda lambda)
      {
         var classified = new AutoHash<IObject, Set>(_ => new Set(), true);
         foreach (var item in set)
         {
            var key = lambda.Invoke(item);
            classified[key].Append(item);
         }

         return new Set(classified.ValueArray().Select(s => (IObject)s).ToArray());
      }

      public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);
   }
}