﻿using System.Collections.Generic;

namespace Kagami.Library.Objects
{
   public class SkipWhileAction : IStreamAction
   {
      Lambda predicate;
      bool skipping;

      public SkipWhileAction(Lambda predicate)
      {
         this.predicate = predicate;
         skipping = true;
      }

      public ILazyStatus Next(ILazyStatus status)
      {
         if (status.IsAccepted && skipping)
         {
            if (predicate.Invoke(status.Object).IsTrue)
               return new Skipped();

            skipping = false;
         }

         return status;
      }

      public IEnumerable<IObject> Execute(IIterator iterator)
      {
         skipping = true;

         foreach (var value in iterator.List())
         {
            if (skipping && predicate.Invoke(value).IsTrue)
               continue;

            skipping = false;
            yield return value;
         }
      }

      public override string ToString() => $"skip while {predicate.Image}";
   }
}