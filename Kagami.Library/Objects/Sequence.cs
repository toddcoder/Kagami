using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
   public class Sequence : Iterator
   {
      int count;
      IObject factor;
      IObject offset;
      int i;

      public Sequence(int count, IObject offset) : base((ICollection)Array.Empty)
      {
         this.count = count;
         factor = Int.IntObject(1);
         this.offset = offset;

         i = 0;
      }

      public Sequence(Sequence sequence, IObject factor) : base((ICollection)Array.Empty)
      {
         count = sequence.count;
         offset = sequence.offset;
         this.factor = factor;

         i = 0;
      }

      public override IMaybe<IObject> Next()
      {
         if (i < count)
         {
            var result = sendMessage(Int.IntObject(i++), "*", factor);
            result = sendMessage(result, "+", offset);
            return result.Some();
         }
         else
            return none<IObject>();
      }
   }
}