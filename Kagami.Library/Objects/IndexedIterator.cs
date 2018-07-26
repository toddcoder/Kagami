using Standard.Types.Maybe;

namespace Kagami.Library.Objects
{
   public class IndexedIterator : Iterator
   {
      public IndexedIterator(ICollection collection) : base(collection) { }

      public override IMaybe<IObject> Next()
      {
         var result = base.Next();
         if (result.If(out var value))
         {
            var newIndex = Int.IntObject(index - 1);
            return Tuple.NewTuple(newIndex, value).Some();
         }
         else
            return result;
      }
   }
}