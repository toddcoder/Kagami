using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class IsPositive : OneNumericOperation
{
   public override Optional<IObject> Execute(Machine machine, INumeric x) => KBoolean.BooleanObject(x.IsPositive).Just();

   public override string ToString() => "is.positive";
}