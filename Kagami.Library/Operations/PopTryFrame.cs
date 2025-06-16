using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class PopTryFrame : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var frameGroup = machine.PopFramesUntil(f => f.FrameType == FrameType.Try);
      if (frameGroup.TryFrame is (true, var tryFrame))
      {
         if (machine.GoTo(tryFrame.Address))
         {
            return nil;
         }
         else
         {
            return badAddress(tryFrame.Address);
         }
      }
      else
      {
         return fail("Can't pop try frame here");
      }
   }

   public override string ToString() => "pop.try.frame";

   public override bool Increment => false;
}