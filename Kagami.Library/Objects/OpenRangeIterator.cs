using System.Collections.Generic;
using Core.Monads;

namespace Kagami.Library.Objects
{
   public class OpenRangeIterator : Iterator
   {
      IObject seed;
      IObject current;
      Lambda lambda;
      OpenRange range;

      public OpenRangeIterator(OpenRange range) : base(range)
      {
         seed = range.Seed;
         current = seed;
         lambda = range.Lambda;
         this.range = range;
      }

      public override bool IsLazy => true;

      public override IMaybe<IObject> Next()
      {
         var result = current;
         current = lambda.Invoke(current);

         return result.Some();
      }

      public override IMaybe<IObject> Peek() => current.Some();

      public override IEnumerable<IObject> List()
      {
         current = seed;
         for (var i = 0; i < int.MaxValue; i++)
         {
            yield return current;
            current = lambda.Invoke(current);
         }
      }

      public override IIterator Clone() => new OpenRangeIterator(range);
   }
}