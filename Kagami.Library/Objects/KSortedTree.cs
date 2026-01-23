using Core.Collections;
using Core.Monads;
using Kagami.Library.Operations;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Objects;

public class KSortedTree : IObject, ICollection
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

   public KBoolean Add(IObject item)
   {
      var added = tree.Add(item);
      if (added)
      {
         _array = nil;
      }

      return added;
   }

   public IObject Remove(IObject item)
   {
      var removed = tree.Remove(item);
      if (removed)
      {
         _array = nil;
      }

      return removed ? Some.Object(item) : KNil.NilValue;
   }

   public string ClassName => "SortedTree";

   public string AsString { get; }
   public string Image { get; }
   public int Hash { get; }
   public bool IsEqualTo(IObject obj) => TODO_IMPLEMENT_ME;

   public bool Match(IObject comparisand, Hash<string, IObject> bindings) => TODO_IMPLEMENT_ME;

   public bool IsTrue { get; }
   public Guid Id { get; init; }

   public IObject this[SkipTake skipTake] => TODO_IMPLEMENT_ME;

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

   public IObject One() => TODO_IMPLEMENT_ME;

   public IObject Copy() => TODO_IMPLEMENT_ME;

   public IIterator Following(IObject following) => TODO_IMPLEMENT_ME;

   public Maybe<TypeConstraint> TypeConstraint { get; }
}