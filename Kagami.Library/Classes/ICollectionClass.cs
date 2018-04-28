using System.Collections.Generic;
using Kagami.Library.Objects;

namespace Kagami.Library.Classes
{
   public interface ICollectionClass
   {
      IObject Revert(IEnumerable<IObject> list);
   }
}