namespace Kagami.Library.Objects;

public class FlatMapAction(Lambda lambda) : IStreamAction
{
   public ILazyStatus Next(ILazyStatus status)
   {
      try
      {
         return status.IsAccepted ? Accepted.New(lambda.Invoke(status.Object)) : status;
      }
      catch (Exception exception)
      {
         return new Failed(exception);
      }
   }

   public IEnumerable<IObject> Execute(IIterator iterator)
   {
      var flattened = (ICollection)iterator.Flatten();
      var newIterator = flattened.GetIterator(false);
      foreach (var item in newIterator.List())
      {
         yield return item;
      }
   }
}