using Core.Collections;

namespace Kagami.Library.Objects
{
   public interface IProcessPlaceholders
   {
      Hash<string, IObject> Passed { get; }

      Hash<string, IObject> Internals { get; }
   }
}