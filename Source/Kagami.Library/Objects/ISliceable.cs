using Core.Monads;

namespace Kagami.Library.Objects
{
   public interface ISliceable
   {
      Slice Slice(ICollection collection);

      Maybe<IObject> Get(IObject index);

      IObject Set(IObject index, IObject value);

      bool ExpandForArray { get; }

      int Length { get; }
   }
}