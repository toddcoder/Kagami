using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class IsNegative : OneNumericOperation
{
   public override Optional<IObject> Execute(Machine machine, INumeric x) => KBoolean.BooleanObject(x.IsNegative).Just();

   public override string ToString() => "is.negative";
}