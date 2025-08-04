using Kagami.Library.Classes;

namespace Kagami.Library.Objects;

public class ByAction(int count, ICollectionClass collectionClass) : IStreamAction
{
   private List<IObject> list = [];

   public ILazyStatus Next(ILazyStatus status)
   {
      if (status.IsAccepted)
      {
         if (list.Count < count)
         {
            list.Add(status.Object);
            return new Skipped();
         }
         else
         {
            var newStatus = new Accepted(collectionClass.Revert(list));
            list.Clear();
            return newStatus;
         }
      }
      else
      {
         return status;
      }
   }

   public IEnumerable<IObject> Execute(IIterator iterator)
   {
      list.Clear();

      foreach (var value in iterator.List())
      {
         if (list.Count < count)
         {
            list.Add(value);
         }
         else
         {
            yield return collectionClass.Revert(list);
            list.Clear();
         }
      }
   }

   public override string ToString() => $"by({count}";
}