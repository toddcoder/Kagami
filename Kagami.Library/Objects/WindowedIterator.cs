using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class WindowedIterator : Iterator
{
   protected int size;
   protected int step;
   protected List<IObject> list;

   public WindowedIterator(ICollection collection, int size, int step) : base(collection)
   {
      this.size = size;
      this.step = step;

      list = collection.GetIterator(false).List().ToList();
   }

   public override Maybe<IObject> Next()
   {
      if (index < list.Count)
      {
         List<IObject> windowed = [];
         for (var i = 0; i < size && index + i < list.Count; i++)
         {
            windowed.Add(list[index + i]);
         }

         index += step;
         return collectionClass.Revert(windowed).Some();
      }
      else
      {
         return nil;
      }
   }

   public override IObject Windowed(int size, int step) => this;
}