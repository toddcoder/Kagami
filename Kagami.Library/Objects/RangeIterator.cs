using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
   public class RangeIterator : Iterator
   {
      protected KRange kRange;
      protected IRangeItem current;
      protected IObject stop;

      public RangeIterator(KRange kRange) : base(kRange)
      {
         this.kRange = kRange;
         current = kRange.Start;
         stop = kRange.StopObj;
      }

      public override Maybe<IObject> Next()
      {
         if (kRange.Compare(current, stop))
         {
            var result = current;
            current = kRange.NextValue(current);
            return result.Object.Some();
         }
         else
         {
            return nil;
         }
      }

      public override Maybe<IObject> Peek() => maybe(kRange.Compare(current, stop), () => current.Object);

      public override IEnumerable<IObject> List()
      {
         while (kRange.Compare(current, stop))
         {
            yield return current.Object;

            current = kRange.NextValue(current);
         }
      }
   }
}