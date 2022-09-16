using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class PopExitFrame : Operation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         var frameGroup = machine.PopFramesUntil(f => f.FrameType == FrameType.Exit);
         if (frameGroup.ExitFrame.If(out var exitFrame))
         {
            if (machine.GoTo(exitFrame.Address))
            {
               return notMatched<IObject>();
            }
            else
            {
               return failedMatch<IObject>(badAddress(exitFrame.Address));
            }
         }
         else
         {
            return "Can't exit here".FailedMatch<IObject>();
         }
      }

      public override bool Increment => false;
   }
}