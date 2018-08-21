using System.Collections.Generic;
using Kagami.Library.Objects;

namespace Kagami.Library.Classes
{
   public class StreamIteratorClass : BaseClass, ICollectionClass
   {
      public override string Name => "StreamIterator";

      public IObject Revert(IEnumerable<IObject> list) => new Array(list);

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         iteratorMessages();
      }

      public BaseClass Equivalent() => new CollectionClass();

      public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
   }
}