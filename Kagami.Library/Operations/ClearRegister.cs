using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class ClearRegister(int index) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      switch (index)
      {
         case 0:
            machine.R0 = nil;
            break;
         case 1:
            machine.R1 = nil;
            break;
         case 2:
            machine.R2 = nil;
            break;
         case 3:
            machine.R3 = nil;
            break;
         default:
            return fail($"Invalid register index {index}");
      }

      return nil;
   }

   public override string ToString() => $"clear.register({index})";
}