using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class Set : IObject, ICollection, IObjectCompare
{
   public static IObject Empty => new Set();

   protected Set<IObject> set;
   protected List<IObject> list;

   public Set(IObject[] items)
   {
      set = [.. items];
      list = [];
   }

   public Set(Set<IObject> set)
   {
      this.set = set;
      list = [];
   }

   public Set(Set otherSet)
   {
      set = otherSet.set;
      list = [];
   }

   public Set(IObject obj)
   {
      set = [obj];
      list = [];
   }

   public Set()
   {
      set = [];
      list = [];
   }

   public string ClassName => "Set";

   public string AsString => set.Select(i => i.AsString).ToString(" ");

   public string Image => $"{{{set.Select(i => i.Image).ToString(", ")}}}";

   public int Hash => set.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is Set s && set.Equals(s.set);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => set.Count > 0;

   public Guid Id { get; init; } = Guid.NewGuid();

   public IIterator GetIterator(bool lazy)
   {
      list = [.. set];
      return lazy ? new LazyIterator(this) : new Iterator(this);
   }

   public Maybe<IObject> Next(int index) => maybe<IObject>() & index < set.Count & (() => list[index]);

   public Maybe<IObject> Peek(int index) => Next(index);

   public Int Length => set.Count;

   public bool ExpandForArray => true;

   public KBoolean In(IObject item) => set.Contains(item);

   public KBoolean NotIn(IObject item) => !set.Contains(item);

   public IObject Times(int count) => this;

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One() => set.Count == 1 ? set.Take(1).First() : this;

   public IObject Copy()
   {
      Set<IObject> newSet = [.. set];
      return new Set(newSet);
   }

   protected void assertNotThisSet(IObject other)
   {
      if (Id == other.Id)
      {
         throw cannotAddSelf();
      }
   }

   public Set Append(IObject item)
   {
      assertNotThisSet(item);
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
         list = [.. set];
         return list[wrapIndex(index, set.Count)];
      }
   }

   public IObject Extend()
   {
      var iterator = GetIterator(false);
      var _next = iterator.Next();
      while (_next is (true, var next))
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
         _ => throw unableToConvert(obj.Image, "Set")
      };
   }

   public KBoolean IsSubsetOf(Set otherSet) => new(set.IsSubsetOf(otherSet.set));

   public KBoolean IsProperSubsetOf(Set otherSet) => new(set.IsProperSubsetOf(otherSet.set));

   public KBoolean IsSupersetOf(Set otherSet) => new(set.IsSupersetOf(otherSet.set));

   public KBoolean IsProperSupersetOf(Set otherSet) => new(set.IsProperSupersetOf(otherSet.set));

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive) => between(this, min, max, inclusive);

   public KBoolean After(IObject min, IObject max, bool inclusive) => after(this, min, max, inclusive);

   public Set XOr(Set other) => Union(other).Difference(Intersection(other));

   public Set Classify(Lambda lambda)
   {
      var classified = new Memo<IObject, Set>.Function(_ => new Set());
      foreach (var item in set)
      {
         var key = lambda.Invoke(item);
         classified[key].Append(item);
      }

      return new Set(classified.GetHash().ValueArray().Select(s => (IObject)s).ToArray());
   }

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);

   public IObject Concatenate(Set otherSet)
   {
      IObject[] result = [.. set, ..otherSet.set];
      return new Set(result);
   }
}