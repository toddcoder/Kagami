using Kagami.Library.Objects;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Collections
{
   public class SequenceIterator : Iterator
   {
      int count;
      int factor;
      int offset;
      int i;

      public SequenceIterator(int count, int factor, int offset) : base((ICollection)Array.Empty)
      {
         this.count = count;
         this.factor = factor;
         this.offset = offset;
         i = 0;
      }

      public override IMaybe<IObject> Next()
      {
         if (i < count)
            return Int.IntObject(i++ * factor + offset).Some();
         else
            return none<IObject>();
      }
   }
}