using System.Collections.Generic;

namespace Kagami.Library.Objects
{
   public class TakeAction : IStreamAction
   {
      int count;
      int index;

      public TakeAction(int count)
      {
         this.count = count;
         index = -1;
      }

      public ILazyStatus Next(ILazyStatus status)
      {
         if (status.IsAccepted)
         {
            if (++index < count)
               return status;
            else
               return new Ended();
         }
         else
            return status;
      }

      public IEnumerable<IObject> Execute(IIterator iterator)
      {
         var i = -1;
         foreach (var value in iterator.List())
         {
            if (++i < count)
               yield return value;

            yield break;
         }
      }

      public override string ToString() => $"take {count}";
   }
}