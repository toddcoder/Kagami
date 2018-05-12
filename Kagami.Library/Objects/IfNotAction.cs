using System;
using System.Collections.Generic;

namespace Kagami.Library.Objects
{
   public class IfNotAction : IStreamAction
   {
      Lambda predicate;

      public IfNotAction(Lambda predicate) => this.predicate = predicate;

      public ILazyStatus Next(ILazyStatus status)
      {
         try
         {
            if (status.IsAccepted)
            {
               if (!predicate.Invoke(status.Object).IsTrue)
                  return status;
               else
                  return new Skipped();
            }
            else
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
            if (!predicate.Invoke(value).IsTrue)
               yield return value;
      }
   }
}