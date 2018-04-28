using System.Collections.Generic;

namespace Kagami.Library.Objects
{
   public interface IStreamAction
   {
      ILazyStatus Next(ILazyStatus status);

      IEnumerable<IObject> Execute(IIterator iterator);
   }
}