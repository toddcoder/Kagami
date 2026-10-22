using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewSequence : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      var sequence = x switch
      {
         Sequence xSequence => xSequence,
         Slip xSlip => new Sequence(xSlip.GetIterator().List()),
         _ => new Sequence([x])
      };

      var _iterator = y switch
      {
         Slip ySlip => ySlip.GetIterator().Some(),
         _ => nil
      };

      if (_iterator is (true, var iterator))
      {
         foreach (var item in iterator.List())
         {
            sequence.Add(item);
         }
      }
      else
      {
         sequence.Add(y);
      }

      return sequence;
   }


   public override string ToString() => "new.sequence";
}