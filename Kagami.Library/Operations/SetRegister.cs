using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class SetRegister(int index) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      switch (index)
      {
         case 0:
            machine.R0 = value.Some();
            break;
         case 1:
            machine.R1 = value.Some();
            break;
         case 2:
            machine.R2 = value.Some();
            break;
         case 3:
            machine.R3 = value.Some();
            break;
         default:
            return fail($"Invalid register index {index}");
      }

      return nil;
   }

   public override string ToString() => $"set.register({index})";
}