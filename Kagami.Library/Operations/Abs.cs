using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class Abs : OneNumericOperation
{
   public override Optional<IObject> Execute(Machine machine, INumeric x) => x.Abs().Just();

   public override string ToString() => "abs";
}