using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class PushSkipFrame : AddressedOperation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         increment = true;

         var frame = new Frame() { Address = address, FrameType = FrameType.Skip };
         machine.PushFrame(frame);

         return notMatched<IObject>();
      }

      public override string ToString() => "push.skip.frame";
   }
}