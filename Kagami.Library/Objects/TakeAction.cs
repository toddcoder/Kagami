using System.Collections.Generic;

namespace Kagami.Library.Objects
{
   public class TakeAction : IStreamAction
   {
      protected int count;
      protected int index;

      public TakeAction(int count)
      {
         this.count = count;
         index = -1;
      }

      public ILazyStatus Next(ILazyStatus status)
      {
         if (status.IsAccepted)
         {
            return ++index < count ? status : new Ended();
         }
         else
         {
	         return status;
         }
      }

      public IEnumerable<IObject> Execute(IIterator iterator)
      {
         var i = -1;
         foreach (var value in iterator.List())
         {
            if (++i < count)
            {
	            yield return value;
            }

            yield break;
         }
      }

      public override string ToString() => $"take {count}";
   }
}