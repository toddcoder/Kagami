using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewMonoTuple : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => new KTuple(value);

   public override string ToString() => "new.mono.tuple";
}