using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Yield : Operation
   {
      public static IMatched<IObject> YieldAction(Machine machine)
      {
         var topFrame = machine.CurrentFrame;
         var frames = machine.PopFrames();
         IMatched<IObject> returnValue;
         if (topFrame.IsEmpty)
            returnValue = None.NoneValue.Matched();
         else if (topFrame.Pop().If(out var popped, out var exception))
            if (popped is None)
               returnValue = popped.Matched();
            else
            {
               IObject copy;
               if (popped is IPristineCopy pc)
                  copy = pc.Copy();
               else
                  copy = popped;
               if (copy is ICopyFields cf)
                  cf.CopyFields(frames.Fields);
               popped = copy;
               returnValue = new YieldReturn(popped, machine.Address, frames).Matched<IObject>();
            }
         else
            return failedMatch<IObject>(exception);

         if (frames.FunctionFrame.If(out var frame))
         {
            var returnAddress = frame.Address;

            if (machine.GoTo(returnAddress))
               return returnValue;
            else
               return failedMatch<IObject>(badAddress(returnAddress));
         }
         else
            return failedMatch<IObject>(invalidStack());
      }

      public override IMatched<IObject> Execute(Machine machine) => YieldAction(machine);

      public override string ToString() => "yield";

      public override bool Increment => false;
   }
}