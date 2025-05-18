using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class IsZero : OneNumericOperation
{
   public override Optional<IObject> Execute(Machine machine, INumeric x) => Boolean.BooleanObject(x.IsZero).Just();

   public override string ToString() => "is.zero";
}