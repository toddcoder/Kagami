﻿using System.Collections.Generic;
using Standard.Types.Maybe;

namespace Kagami.Library.Objects
{
   public class OpenRangeIterator : LazyIterator
   {
      IObject seed;
      IObject current;
      Lambda lambda;

      public OpenRangeIterator(OpenRange range) : base(range)
      {
         seed = range.Seed;
         current = seed;
         lambda = range.Lambda;
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
   }
}