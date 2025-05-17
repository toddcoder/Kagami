using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class PushSkipFrame : AddressedOperation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      increment = true;

      var frame = new Frame { Address = address, FrameType = FrameType.Skip };
      machine.PushFrame(frame);

      return nil;
   }

   public override string ToString() => "push.skip.frame";
}