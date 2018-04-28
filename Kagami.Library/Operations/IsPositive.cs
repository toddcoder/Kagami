using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class IsPositive : OneNumericOperation
   {
      public override IMatched<IObject> Execute(Machine machine, INumeric x) => Boolean.Object(x.IsPositive).Matched();

      public override string ToString() => "is.positive";
   }
}