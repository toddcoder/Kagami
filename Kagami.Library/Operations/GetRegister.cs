using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class GetRegister(int index) : Operation
{
   public override Optional<IObject> Execute(Machine machine) => index switch
   {
      0 => machine.R0.Optional(),
      1 => machine.R1.Optional(),
      2 => machine.R2.Optional(),
      3 => machine.R3.Optional(),
      _ => fail($"Invalid register index {index}")
   };

   public override string ToString() => $"get.register({index})";
}