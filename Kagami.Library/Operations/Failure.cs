using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class Failure : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => Objects.Failure.Object(value.AsString).Just();

   public override string ToString() => "failure";
}