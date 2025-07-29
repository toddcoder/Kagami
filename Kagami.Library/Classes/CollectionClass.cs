using Kagami.Library.Objects;

namespace Kagami.Library.Classes
{
   public class CollectionClass : BaseClass
   {
      public override string Name => "Collection";

      public override bool MatchCompatible(BaseClass otherClass) => otherClass is IIterator or ICollectionClass;

      public override bool AssignCompatible(BaseClass otherClass) => MatchCompatible(otherClass);

      public override IObject DefaultValue => KArray.Empty;
   }
}