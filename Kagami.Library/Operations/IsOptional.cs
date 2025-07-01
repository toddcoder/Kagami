using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class IsOptional : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => KBoolean.BooleanObject(value is Objects.Some or None).Just();

   public override string ToString() => "is.optional";
}