using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.CollectionFunctions;

namespace Kagami.Library.Objects;

public class FlatMapIterator : Iterator
{
   public FlatMapIterator(ICollection collection) : base(collection)
   {
   }

   public override IEnumerable<IObject> List() => list(this);

   protected static IEnumerable<IObject> list(IIterator iterator)
   {
      while (iterator.Next() is (true, var next))
      {
         if (next is ICollection innerCollection)
         {
            var innerIterator = new FlatMapIterator(innerCollection);
            foreach (var item in innerIterator.List())
            {
               yield return item;
            }
         }
         else
         {
            yield return next;
         }
      }
   }

   public override IObject FlatMap(Lambda lambda)
   {
      List<IObject> innerList = [];
      foreach (var item in List())
      {
         var result = lambda.Invoke(item);
         innerList.AddRange(getEnumerable(result));
      }

      return collectionClass.Revert(innerList, nil);
   }
}