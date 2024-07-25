using Kagami.Library.Objects;

namespace Kagami.Library.Classes
{
   public class CollectionClass : BaseClass
   {
      public override string Name => "Collection";

      public override bool MatchCompatible(BaseClass otherClass) => otherClass is IIterator || otherClass is ICollectionClass;

      public override bool AssignCompatible(BaseClass otherClass) => MatchCompatible(otherClass);
   }
}