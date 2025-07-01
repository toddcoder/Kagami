using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class TryBegin : AddressedOperation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      increment = true;

      var frame = new Frame { Address = address, FrameType = FrameType.Try };
      machine.PushFrame(frame);

      return nil;
   }

   public override string ToString() => "try.begin";
}