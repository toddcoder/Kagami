using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewSlip : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => new Slip(value);

   public override string ToString() => "new.slip";
}