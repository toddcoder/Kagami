using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
   public class IsPositive : OneNumericOperation
   {
      public override IMatched<IObject> Execute(Machine machine, INumeric x) => Boolean.BooleanObject(x.IsPositive).Matched();

      public override string ToString() => "is.positive";
   }
}