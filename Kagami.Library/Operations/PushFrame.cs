using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class PushFrame : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      machine.PushFrame(new Frame());
      return nil;
   }

   public override string ToString() => "push.frame";
}