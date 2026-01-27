using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects;

public class KSortedTree : IObject, ICollection, IMutableCollection
{
   protected readonly SortedTree tree;
   protected Maybe<IObject[]> _array = nil;

   public KSortedTree()
   {
      tree = new SortedTree();
   }

   public KSortedTree(IEnumerable<IObject> items) : this()
   {
      foreach (var item in items)
      {
         tree.Add(item);
      }
   }

   public KBoolean Append(IObject item)
   {
      var added = tree.Add(item);
      if (added)
      {
         _array = nil;
      }

      return added;
   }

   IObject IMutableCollection.Append(IObject obj) => Append(obj);

   public IObject Remove(IObject item)
   {
      var removed = tree.Remove(item);
      if (removed)
      {
         _array = nil;
      }

      return removed ? Some.Object(item) : KNil.NilValue;
   }

   public IObject RemoveAt(int index) => TODO_IMPLEMENT_ME;

   public IObject RemoveAll(IObject obj) => TODO_IMPLEMENT_ME;

   public IObject InsertAt(int index, IObject obj) => TODO_IMPLEMENT_ME;

   public KBoolean IsEmpty { get; }
   public KBoolean IsNotEmpty { get; }
   public IObject Assign(SkipTake skipTake, IEnumerable<IObject> values) => TODO_IMPLEMENT_ME;

   public IObject Prepend(IObject obj) => TODO_IMPLEMENT_ME;

   public IObject Clear() => TODO_IMPLEMENT_ME;

   public string ClassName => "SortedTree";

   public string AsString => tree.Select(i => i.AsString).ToString(", ");

   public string Image => $"[+{tree.Select(i => i.Image).ToString(", ")}]";

   public int Hash => tree.GetHashCode();

   public bool IsEqualTo(IObject obj) => obj is KSortedTree kSortedTree && kSortedTree.tree.Equals(tree);

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

   public bool IsTrue => tree.Count > 0;

   public Guid Id { get; init; }

   public IObject this[SkipTake skipTake] => CollectionFunctions.skipTake(this, skipTake);

   public IIterator GetIterator(bool lazy) => lazy ? new Iterator(this) : new LazyIterator(this);

   public Maybe<IObject> Next(int index)
   {
      (var array, _array) = _array.Create(() => [.. tree]);
      return array[index].Some();
   }

   public Maybe<IObject> Peek(int index) => Next(index);

   public Int Length => tree.Count;

   public bool ExpandForArray => false;

   public KBoolean In(IObject item) => tree.Contains(item);

   public KBoolean NotIn(IObject item) => !tree.Contains(item);

   public IObject Times(int count) => this;

   public KString MakeString(string connector) => makeString(this, connector);

   public IIterator GetIndexedIterator() => new IndexedIterator(this);

   public IObject One() => tree.Count == 1 ? tree.FirstOrFailure() | KUnit.Value : this;

   public IObject Copy() => new KSortedTree(tree);

   public IIterator Following(IObject following) => GetIterator(false);

   public Maybe<TypeConstraint> TypeConstraint => nil;
}