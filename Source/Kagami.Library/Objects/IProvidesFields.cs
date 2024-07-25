using Kagami.Library.Runtime;

namespace Kagami.Library.Objects
{
   public interface IProvidesFields
   {
      bool ProvidesFields { get; }

      Fields Fields { get; }
   }
}