using Core.Monads;

namespace Kagami.Library.Objects;

public class IndexedIterator : Iterator
{
   public IndexedIterator(ICollection collection) : base(collection)
   {
   }

   public override Maybe<IObject> Next()
   {
      var _next = base.Next();
      if (_next is (true, var value))
      {
         var newIndex = Int.IntObject(index - 1);
         var x = new NameValue("index", newIndex);
         var y = new NameValue("value", value);

         return Tuple.NewTuple(x, y).Some();
      }
      else
      {
         return _next;
      }
   }
}