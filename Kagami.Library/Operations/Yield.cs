using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using Core.Monads.Lazy;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class Yield : Operation
{
   public static Optional<IObject> YieldAction(Machine machine)
   {
      var topFrame = machine.CurrentFrame;
      var frames = machine.PopFrames();
      Optional<IObject> _value;
      LazyResult<IObject> _popped = nil;
      if (topFrame.IsEmpty)
      {
         _value = None.NoneValue.Just();
      }
      else if (_popped.ValueOf(topFrame.Pop()) is (true, var popped))
      {
         if (popped is None)
         {
            _value = popped.Just();
         }
         else
         {
            IObject copy;
            if (popped is IPristineCopy pristineCopy)
            {
               copy = pristineCopy.Copy();
            }
            else
            {
               copy = popped;
            }

            if (copy is ICopyFields cf)
            {
               cf.CopyFields(frames.Fields);
            }

            popped = copy;
            _value = new YieldReturn(popped, machine.Address, frames);
         }
      }
      else
      {
         return _popped.Exception;
      }

      if (frames.FunctionFrame is (true, var frame))
      {
         var returnAddress = frame.Address;

         if (machine.GoTo(returnAddress))
         {
            return _value;
         }
         else
         {
            return badAddress(returnAddress);
         }
      }
      else
      {
         return invalidStack("frame");
      }
   }

   public override Optional<IObject> Execute(Machine machine) => YieldAction(machine);

   public override string ToString() => "yield";

   public override bool Increment => false;
}