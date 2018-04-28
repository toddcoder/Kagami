using System;
using System.Collections.Generic;

namespace Kagami.Library.Objects
{
   public class IfAction : IStreamAction
   {
      Lambda predicate;

      public IfAction(Lambda predicate) => this.predicate = predicate;

      public ILazyStatus Next(ILazyStatus status)
      {
         try
         {
            if (status.IsAccepted)
            {
               if (predicate.Invoke(status.Object).IsTrue)
                  return status;

               return new Skipped();
            }

            return status;
         }
         catch (Exception exception)
         {
            return new Failed(exception);
         }
      }

      public IEnumerable<IObject> Execute(IIterator iterator)
      {
         foreach (var value in iterator.List())
            if (predicate.Invoke(value).IsTrue)
               yield return value;
      }

      public override string ToString() => $"if {predicate.Image}";
   }
}