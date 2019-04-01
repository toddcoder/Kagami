using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class PopSkipFrame : Operation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         var frameGroup = machine.PopFramesUntil(f => f.FrameType == FrameType.Skip);
         if (frameGroup.SkipFrame.If(out var skipFrame))
            if (machine.GoTo(skipFrame.Address))
               return notMatched<IObject>();
            else
               return failedMatch<IObject>(badAddress(skipFrame.Address));
         else
            return "Can't skip here".FailedMatch<IObject>();
      }

      public override bool Increment => false;

      public override string ToString() => "pop.skip.frame";
   }
}