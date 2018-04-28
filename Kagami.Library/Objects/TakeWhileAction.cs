using System.Collections.Generic;

namespace Kagami.Library.Objects
{
   public class TakeWhileAction : IStreamAction
   {
      Lambda predicate;
      bool taking;

      public TakeWhileAction(Lambda predicate)
      {
         this.predicate = predicate;
         taking = true;
      }

      public ILazyStatus Next(ILazyStatus status)
      {
         if (status.IsAccepted && taking)
         {
            if (predicate.Invoke(status.Object).IsTrue)
               return status;

            taking = false;
         }

         return new Ended();
      }

      public IEnumerable<IObject> Execute(IIterator iterator)
      {
         foreach (var value in iterator.List())
            if (predicate.Invoke(value).IsTrue)
               yield return value;
            else
               yield break;
      }

      public override string ToString() => $"take while {predicate.Image}";
   }
}