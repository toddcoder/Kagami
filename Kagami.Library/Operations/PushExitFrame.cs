using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class PushExitFrame : AddressedOperation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         increment = true;

         var frame = new Frame() { Address = address, FrameType = FrameType.Exit };
         machine.PushFrame(frame);

         return notMatched<IObject>();
      }

      public override string ToString() => "push.exit.frame";
   }
}