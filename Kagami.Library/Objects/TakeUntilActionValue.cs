namespace Kagami.Library.Objects;

public class TakeUntilActionValue(IObject obj) : IStreamAction
{
   protected bool taking = true;

   public ILazyStatus Next(ILazyStatus status)
   {
      if (status.IsAccepted && taking)
      {
         if (!status.Object.IsEqualTo(obj))
         {
            return status;
         }

         taking = false;
      }

      return new Ended();
   }

   public IEnumerable<IObject> Execute(IIterator iterator)
   {
      foreach (var value in iterator.List())
      {
         if (!value.IsEqualTo(obj))
         {
            yield return value;
         }
         else
         {
            yield break;
         }
      }
   }
}