using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class PopSkipFrame : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var frameGroup = machine.PopFramesUntil(f => f.FrameType == FrameType.Skip);
      if (frameGroup.SkipFrame is (true, var skipFrame))
      {
         skipFrame.ExecuteDeferred(machine);
         if (machine.GoTo(skipFrame.Address))
         {
            return nil;
         }
         else
         {
            return badAddress(skipFrame.Address);
         }
      }
      else
      {
         return fail("Can't skip here");
      }
   }

   public override bool Increment => false;

   public override string ToString() => "pop.skip.frame";
}