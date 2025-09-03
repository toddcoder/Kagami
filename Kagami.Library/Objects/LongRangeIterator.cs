using Core.Monads;
using System.Numerics;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class LongRangeIterator : Iterator
{
   protected LongRange range;
   protected BigInteger current;
   protected BigInteger stop;

   public LongRangeIterator(LongRange range) : base(range)
   {
      this.range = range;
      current = this.range.Start.Value;
      stop = this.range.Stop.Value;
   }

   public override Maybe<IObject> Next()
   {
      if (range.Compare(current, (Long)stop))
      {
         var result = current;
         current = range.NextValue(current).Value;

         return Long.LongObject(result).Some();
      }
      else
      {
         return nil;
      }
   }

   public override Maybe<IObject> Peek() => maybe<IObject>() & range.Compare(current, (Long)stop) & (() => Long.LongObject(current));

   public override IEnumerable<IObject> List()
   {
      while (range.Compare(current, Long.LongObject(stop)))
      {
         yield return Long.LongObject(current);

         current = range.NextValue(current).Value;
      }
   }
}