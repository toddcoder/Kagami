using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class PushFunctionFrame : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      machine.PushFrame(new Frame { FrameType = FrameType.Function });
      return nil;
   }

   public override string ToString() => "push.function.frame";
}