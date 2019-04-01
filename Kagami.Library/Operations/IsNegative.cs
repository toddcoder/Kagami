using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class IsNegative : OneNumericOperation
   {
      public override IMatched<IObject> Execute(Machine machine, INumeric x) => Boolean.BooleanObject(x.IsNegative).Matched();

      public override string ToString() => "is.negative";
   }
}