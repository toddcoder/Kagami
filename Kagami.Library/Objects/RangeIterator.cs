using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
   public class RangeIterator : Iterator
   {
      Range range;
      IRangeItem current;
      IObject stop;

      public RangeIterator(Range range) : base(range)
      {
         this.range = range;
         current = range.Start;
         stop = range.StopObj;
      }

      public override IMaybe<IObject> Next()
      {
         if (range.Compare(current, stop))
         {
            var result = current;
            current = range.NextValue(current);
            return result.Object.Some();
         }
         else
            return none<IObject>();
      }

      public override IMaybe<IObject> Peek()
      {
         if (range.Compare(current, stop))
            return current.Object.Some();
         else
            return none<IObject>();
      }

      public override IEnumerable<IObject> List()
      {
         while (range.Compare(current, stop))
         {
            yield return current.Object;

            current = range.NextValue(current);
         }
      }
   }
}