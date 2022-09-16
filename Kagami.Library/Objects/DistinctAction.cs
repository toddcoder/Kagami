using System.Collections.Generic;
using Core.Collections;

namespace Kagami.Library.Objects
{
   public class DistinctAction : IStreamAction
   {
      protected Set<IObject> existing;

      public DistinctAction() => existing = new Set<IObject>();

      public ILazyStatus Next(ILazyStatus status)
      {
         if (status.IsAccepted)
         {
            if (existing.Contains(status.Object))
            {
               return new Skipped();
            }
            else
            {
               existing.Add(status.Object);
            }
         }

         return status;
      }

      public IEnumerable<IObject> Execute(IIterator iterator)
      {
         existing.Clear();
         foreach (var value in iterator.List())
         {
            if (!existing.Contains(value))
            {
               existing.Add(value);
               yield return value;
            }
         }
      }

      public override string ToString() => "distinct";
   }
}