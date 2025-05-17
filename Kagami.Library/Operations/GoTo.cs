using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class GoTo : AddressedOperation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      return machine.GoTo(address) ? nil : badAddress(address);
   }

   public override string ToString() => $"goto({address})";
}