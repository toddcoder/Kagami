using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class IsResult : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) =>
      KBoolean.BooleanObject(value is Objects.Success or Objects.Failure).Just();

   public override string ToString() => "is.result";
}