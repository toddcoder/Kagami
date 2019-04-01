using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class GoTo : AddressedOperation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.GoTo(address))
            return notMatched<IObject>();
         else
            return failedMatch<IObject>(badAddress(address));
      }

      public override string ToString() => $"goto({address})";
   }
}