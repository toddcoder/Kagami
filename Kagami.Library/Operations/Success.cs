using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class Success : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => Objects.Success.Object(value).Just();

   public override string ToString() => "success";
}