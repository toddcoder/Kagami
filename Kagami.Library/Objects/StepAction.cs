using System.Collections.Generic;

namespace Kagami.Library.Objects
{
   public class StepAction : IStreamAction
   {
      int step;
      int current;

      public StepAction(int step)
      {
         this.step = step;
         current = 0;
      }

      public ILazyStatus Next(ILazyStatus status)
      {
         if (status.IsAccepted)
         {
            if (current++ < step - 1)
            {
               return new Skipped();
            }
            else
            {
               current = 0;
               return new Accepted(status.Object);
            }
         }
         else
         {
            return status;
         }
      }

      public IEnumerable<IObject> Execute(IIterator iterator)
      {
         while (true)
         {
            for (var i = 0; i < step - 1; i++)
            {
               if (iterator.Next().HasValue) { }
               else
               {
                  yield break;
               }
            }

            if (iterator.Next().If(out var item))
            {
               yield return item;
            }
            else
            {
               yield break;
            }
         }
      }
   }
}