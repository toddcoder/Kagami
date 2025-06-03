using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class PopAddress : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      machine.PopAddress();
      return nil;
   }

   public override string ToString() => "pop.address";
}