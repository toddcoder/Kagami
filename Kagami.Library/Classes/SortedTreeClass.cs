using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class SortedTreeClass : BaseClass, ICollectionClass
{
   public override string Name => "SortedTree";

   public override IObject DefaultValue => new KSortedTree();

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Collection");

   public IObject Revert(IEnumerable<IObject> list, Maybe<TypeConstraint> _typeConstraint) => new KSortedTree(list);

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();
      mutableCollectionMessages();
   }
}