using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class IsTrue : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => KBoolean.BooleanObject(value.IsTrue).Just();

   public override string ToString() => "is.true";
}