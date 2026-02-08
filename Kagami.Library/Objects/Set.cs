using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Kagami.Library.Classes;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class Set : IObject, ICollection, IObjectCompare, IMutable, ITypedCollection
{
   public static Set Empty => new();

   protected Set<IObject> set = [];
   protected List<IObject> list = [];

   protected void assertIncomingValueIsEquivalent(IObject value)
   {
      if (TypeConstraint is (true, var typeConstraint))
      {
         if (!typeConstraint.Matches(classOf(value)))
         {
            throw fail($"{value.AsString} is incompatible with {typeConstraint.AsString}");
         }
      }
   }

   protected void assertIncomingValuesAreEquivalent(IEnumerable<IObject> values)
   {
      foreach (var value in values)
      {
         assertIncomingValueIsEquivalent(value);
      }
   }

   protected void assertValuesAreEquivalent()
   {
      foreach (var value in list)
      {
         assertIncomingValueIsEquivalent(value);
      }
   }

   protected void assertTypeConstraintIsEquivalent(TypeConstraint otherTypeConstraint)
   {
      if (TypeConstraint is (true, var typeConstraint))
      {
         if (!typeConstraint.IsEquivalentTo(otherTypeConstraint))
         {
            throw fail($"{typeConstraint.AsString} is incompatible with {otherTypeConstraint.AsString}");
         }
      }
   }

   public Set(IObject[] items)
   {
      set = [];
      foreach (var obj in items)
      {
         if (obj is KRange range)
         {
            set.AddRange(range.GetIterator(false).List());
         }
         else
         {
            set.Add(obj);
         }
      }
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

   public string Image => $"{{{set.Select(i => i.Image).ToString(", ")}}}" + (TypeConstraint.Map(tc => $" {tc.Image}") | "");

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

   public Maybe<IObject> Next(int index) => maybe<IObject>() & index < list.Count & (() => list[index]);

   public Maybe<IObject> Peek(int index) => Next(index);

   public Int Length => set.Count;

   public bool ExpandForArray => true;

   public KBoolean In(IObject item)
   {
      assertIncomingValueIsEquivalent(item);
      return set.Contains(item);
   }

   public KBoolean NotIn(IObject item)
   {
      assertIncomingValueIsEquivalent(item);
      return !set.Contains(item);
   }

   public IObject Times(int count) => this;

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator()
   {
      list = [.. set];
      return new IndexedIterator(this);
   }

   public IObject One() => set.Count == 1 ? set.Take(1).First() : this;

   public IObject Copy()
   {
      Set<IObject> newSet = [.. set];
      return new Set(newSet) { TypeConstraint = TypeConstraint };
   }

   public IIterator Following(IObject following) => new MultiIterator(this, following);

   public Maybe<TypeConstraint> TypeConstraint
   {
      get;
      set
      {
         assertValuesAreEquivalent();
         field = value;
      }
   } = nil;

   public IObject SetType(TypeConstraint typeConstraint)
   {
      TypeConstraint = typeConstraint;
      return this;
   }

   public IObject AutoType()
   {
      if (list.Count == 0)
      {
         return this;
      }

      var type = list[0].ClassName;
      for (var i = 1; i < list.Count; i++)
      {
         if (list[i].ClassName != type)
         {
            return this;
         }
      }

      TypeConstraint = Objects.TypeConstraint.FromList(type);
      return this;
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
      assertIncomingValueIsEquivalent(item);
      assertNotThisSet(item);
      set.Add(item);

      return this;
   }

   public Set Remove(IObject item)
   {
      assertIncomingValueIsEquivalent(item);
      set.Remove(item);

      return this;
   }

   public IObject RemoveAndReturn(IObject item)
   {
      assertIncomingValueIsEquivalent(item);
      set.Remove(item);

      return set.Contains(item) ? KNil.NilValue : Some.Object(item);
   }

   public Set Union(Set other) => new(set.Union(other.set)) { TypeConstraint = TypeConstraint };

   public Set Difference(Set other) => new((IObject[])[..set.Except(other.set)]) { TypeConstraint = TypeConstraint };

   public Set Intersection(Set other) => new(set.Intersection(other.set)) { TypeConstraint = TypeConstraint };

   public IObject this[int index]
   {
      get
      {
         list = [.. set];
         return list[wrapIndex(index, set.Count)];
      }
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

   public KBoolean Overlaps(Set otherSet) => new(set.Overlaps(otherSet.set));

   public KBoolean IsDisjointWith(Set otherSet) => new(set.IsDisjointWith(otherSet.set));

   public IObject Object => this;

   public KBoolean Between(IObject min, IObject max, bool inclusive)
   {
      assertIncomingValueIsEquivalent(min);
      assertIncomingValueIsEquivalent(max);

      return between(this, min, max, inclusive);
   }

   public KBoolean After(IObject min, IObject max, bool inclusive)
   {
      assertIncomingValueIsEquivalent(min);
      assertIncomingValueIsEquivalent(max);

      return after(this, min, max, inclusive);
   }

   public Set XOr(Set other) => Union(other).Difference(Intersection(other));

   public Set Classify(Lambda lambda)
   {
      var classified = new Memo<IObject, Set>.Function(_ => new Set());
      foreach (var item in set)
      {
         var key = lambda.Invoke(item);
         classified[key].Append(item);
      }

      return new Set((IObject[])[.. classified.GetHash().ValueArray().Select(IObject (s) => s)]);
   }

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);

   public IObject Concatenate(Set otherSet)
   {
      IObject[] result = [.. set, ..otherSet.set];
      return new Set(result) { TypeConstraint = TypeConstraint };
   }

   public Set Extend(IObject obj)
   {
      if (obj is ICollection collection)
      {
         foreach (var item in collection.GetIterator(false).List())
         {
            assertIncomingValueIsEquivalent(item);
            Append(item);
         }
      }
      else
      {
         Append(obj);
      }

      return this;
   }

   public Set Retain(Lambda lambda)
   {
      Set<IObject> newSet = [];
      newSet.AddRange(set.Where(item => lambda.Invoke(item).IsTrue));

      set = newSet;
      list = [.. set];

      return this;
   }

   public Set Remove(Lambda lambda)
   {
      Set<IObject> newSet = [];
      newSet.AddRange(set.Where(item => !lambda.Invoke(item).IsTrue));

      set = newSet;
      list = [.. set];

      return this;
   }
}