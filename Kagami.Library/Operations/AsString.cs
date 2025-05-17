using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class AsString : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => String.StringObject(value.AsString).Just();

   public override string ToString() => "string";
}