using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class PopExitFrame : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var frameGroup = machine.PopFramesUntil(f => f.FrameType == FrameType.Exit);
      if (frameGroup.ExitFrame is (true, var exitFrame))
      {
         if (machine.GoTo(exitFrame.Address))
         {
            return nil;
         }
         else
         {
            return badAddress(exitFrame.Address);
         }
      }
      else
      {
         return fail("Can't exit here");
      }
   }

   public override bool Increment => false;
}