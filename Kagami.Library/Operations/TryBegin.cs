using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class TryBegin : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      machine.PushFrame(Frame.TryFrame());
      return nil;
   }

   public override string ToString() => "try.begin";
}