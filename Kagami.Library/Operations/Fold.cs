using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.AttemptFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Operations.OperationFunctions;

namespace Kagami.Library.Operations;

public class Fold(bool left) : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      if (x is not KTuple && canIterate(x))
      {
         var _iterator = getIterator(x, false);
         if (_iterator is (true, var iterator))
         {
            var objectIterator = (IObject)iterator;
            if (y is Lambda lambda)
            {
               Selector selector = left ? "foldl(_<Lambda>)" : "foldr(_<Lambda>)";
               return tryTo(() => classOf(objectIterator).SendMessage(objectIterator, selector, new Arguments(lambda))).Optional();
            }
            else
            {
               var tuple = new KTuple(objectIterator, y);
               return tuple;
            }
         }
         else
         {
            return _iterator.Exception;
         }
      }
      else if (x is KTuple tuple && y is Lambda lambda)
      {
         var iterator = tuple[0];
         var initialValue = tuple[1];
         Selector selector = left ? "foldl(_,_<Lambda>)" : "foldr(_,_<Lambda>)";
         return tryTo(() => classOf(iterator).SendMessage(iterator, selector, new Arguments(initialValue, lambda))).Optional();
      }
      else
      {
         return incompatibleClasses(x, "Collection or Tuple");
      }
   }

   public override string ToString() => $"fold.{(left ? "left" : "right")}";
}