namespace Kagami.Library.Objects;

public class TakeUntilAction(Lambda predicate) : IStreamAction
{
   protected bool taking = true;

   public ILazyStatus Next(ILazyStatus status)
   {
      if (status.IsAccepted && taking)
      {
         if (!predicate.Invoke(status.Object).IsTrue)
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
         if (!predicate.Invoke(value).IsTrue)
         {
            yield return value;
         }
         else
         {
            yield break;
         }
      }
   }

   public override string ToString() => $"take while {predicate.Image}";
}