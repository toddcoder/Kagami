using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class WindowedIterator : Iterator
{
   protected int size;
   protected int step;
   protected bool partial;
   protected List<IObject> list;

   public WindowedIterator(ICollection collection, int size, int step, bool partial) : base(collection)
   {
      this.size = size;
      this.step = step;
      this.partial = partial;

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
         return windowed.Count == size || partial ? collectionClass.Revert(windowed, nil).Some() : nil;
      }
      else
      {
         return nil;
      }
   }

   public override IObject Windowed(int size, int step, bool partial) => this;
}