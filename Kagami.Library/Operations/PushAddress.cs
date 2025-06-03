using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class PushAddress : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      machine.PushAddress();
      return nil;
   }

   public override string ToString() => "push.address";
}