using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class GetX : Operation
{
   public override Optional<IObject> Execute(Machine machine) => machine.X.Just();

   public override string ToString() => "get.x";
}