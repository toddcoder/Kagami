using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using Core.Monads.Lazy;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewSequence : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      LazyResult<IObject> _x = nil;
      var _y = machine.Pop();
      if (_y is (true, var y))
      {
         if (machine.IsEmpty)
         {
            return new KTuple(y);
         }
         else if (_x.ValueOf(machine.Pop()) is (true, var x))
         {
            if (x is Sequence sequence)
            {
               if (y is Slip slip)
               {
                  var collection = (ICollection)slip.GetIterator().Flatten();
                  foreach (var obj in collection.GetIterator(false).List())
                  {
                     sequence.Add(obj);
                  }

                  return sequence;
               }

               sequence.Add(y);
               return sequence;
            }
            else
            {
               if (y is Slip slip)
               {
                  var collection = (ICollection)slip.GetIterator().Flatten();
                  List<IObject> list = [.. collection.GetIterator(false).List()];
                  if (list.Count > 1)
                  {
                     var newSequence = new Sequence(x, list[0]);
                     foreach (var obj in list.Skip(1))
                     {
                        newSequence.Add(obj);
                     }

                     return newSequence;
                  }
                  else
                  {
                     return new Sequence(x, list[0]);
                  }
               }
               return new Sequence(x, y);
            }
         }
         else
         {
            return _x.Exception;
         }
      }
      else
      {
         return _y.Exception;
      }
   }

   public override string ToString() => "new.sequence";
}