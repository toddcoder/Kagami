namespace Kagami.Library.Objects;

public class IfNotAction(Lambda predicate) : IStreamAction
{
   public ILazyStatus Next(ILazyStatus status)
   {
      try
      {
         if (status.IsAccepted)
         {
            return !predicate.Invoke(status.Object).IsTrue ? status : new Skipped();
         }
         else
         {
            return status;
         }
      }
      catch (Exception exception)
      {
         return new Failed(exception);
      }
   }

   public IEnumerable<IObject> Execute(IIterator iterator)
   {
      foreach (var value in iterator.List())
      {
         if (!predicate.Invoke(value).IsTrue)
         {
            yield return value;
         }
      }
   }

   public override string ToString() => $"if not {predicate.Image}";
}