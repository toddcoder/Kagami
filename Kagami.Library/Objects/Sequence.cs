﻿using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
   public class Sequence : Iterator
   {
      protected int count;
      protected IObject factor;
      protected IObject offset;
      protected int i;

      public Sequence(int count, IObject offset) : base((ICollection)KArray.Empty)
      {
         this.count = count;
         factor = Int.IntObject(1);
         this.offset = offset;

         i = 0;
      }

      public Sequence(Sequence sequence, IObject factor) : base((ICollection)KArray.Empty)
      {
         count = sequence.count;
         offset = sequence.offset;
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
}