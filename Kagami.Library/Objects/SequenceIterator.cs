using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class SequenceIterator : Iterator
{
   protected int count;
   protected IObject factor;
   protected IObject offset;
   protected int i;

   public SequenceIterator(int count, IObject offset) : base((ICollection)KArray.Empty)
   {
      this.count = count;
      factor = Int.IntObject(1);
      this.offset = offset;

      i = 0;
   }

   public SequenceIterator(SequenceIterator sequenceIterator, IObject factor) : base((ICollection)KArray.Empty)
   {
      count = sequenceIterator.count;
      offset = sequenceIterator.offset;
      this.factor = factor;

      i = 0;
   }

   public override Maybe<IObject> Next()
   {
      if (i < count)
      {
         var result = sendMessage(Int.IntObject(i++), "*", factor);
         result = sendMessage(result, "+", offset);
         return result.Some();
      }
      else
      {
         return nil;
      }
   }
}