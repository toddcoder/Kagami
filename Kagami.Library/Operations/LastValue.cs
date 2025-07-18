using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class LastValue : Operation
{
   public override Optional<IObject> Execute(Machine machine) => machine.LastValue.Just();

   public override string ToString() => "last.value";
}