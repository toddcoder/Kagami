using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects;

public class ChunkedIterator : Iterator
{
   protected int count;
   protected List<IObject> list;

   public ChunkedIterator(ICollection collection, int count) : base(collection)
   {
      this.count = count;
      list = collection.GetIterator(false).List().ToList();
   }

   public override Maybe<IObject> Next()
   {
      if (index < list.Count)
      {
         List<IObject> chunk = [];
         for (var i = 0; i < count && index + i < list.Count; i++)
         {
            chunk.Add(list[index + i]);
         }

         index += count;
         return collectionClass.Revert(chunk).Some();
      }
      else
      {
         return nil;
      }
   }

   public override IObject Chunked(int count) => this;
}