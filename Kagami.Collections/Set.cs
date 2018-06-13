using System.Linq;
using Kagami.Library.Objects;
using Standard.Types.Collections;
using Standard.Types.Enumerables;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Collections
{
   public class Set : IObject, ICollection, IObjectCompare
   {
      Set<IObject> set;

      public Set(IObject[] items) => set = new Set<IObject>(items);

      public Set(Set<IObject> set) => this.set = set;

      public Set(Set otherSet) => set = otherSet.set;

      public Set() => set = new Set<IObject>();

      public string ClassName => "Set";

      public string AsString => set.Select(i => i.AsString).Listify(" ");

      public string Image => $"Set({set.Select(i => i.Image).Listify()})";

      public int Hash => set.GetHashCode();

      public bool IsEqualTo(IObject obj) => obj is Set s && set.Equals(s.set);

      public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

      public bool IsTrue => set.Count > 0;

      public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

      public IMaybe<IObject> Next(int index) => when(index < set.Count, () => set[index]);

      public IMaybe<IObject> Peek(int index) => Next(index);

      public Int Length => set.Count;

      public bool ExpandForArray => true;

      public Boolean In(IObject item) => set.Contains(item);

      public Boolean NotIn(IObject item) => !set.Contains(item);

      public IObject Times(int count) => this;

      public IObject Flatten() => flatten(this);

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

      public Set Union(Set other) => new Set(set.Union(other.set));

      public Set Difference(Set other) => new Set(set.Except(other.set).ToArray());

      public Set Intersection(Set other) => new Set(set.Intersection(other.set));

      public IObject this[int index] => set[wrapIndex(index, set.Count)];

      public IObject Extend(IObject obj)
      {
         var iterator = GetIterator(false);
         var next = iterator.Next();
         while (next.If(out var value))
         {
            Append(value);
            next = iterator.Next();
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
         switch (obj)
         {
            case Set otherSet when set.IsStrictSubsetOf(otherSet.set):
               return -1;
            case Set otherSet when set.IsSubsetOf(otherSet.set):
               return 0;
            case Set _:
               return 1;
            default:
               throw unableToConvert(obj.Image, "Set");
         }
      }

      public IObject Object => this;
   }
}