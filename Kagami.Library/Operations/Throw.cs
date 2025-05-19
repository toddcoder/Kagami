using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Throw : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => fail(value.AsString);

   public override string ToString() => "throw";
}