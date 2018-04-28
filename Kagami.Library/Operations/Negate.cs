using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class Negate : OneNumericOperation
   {
      public override IMatched<IObject> Execute(Machine machine, INumeric x)
      {
         switch (x)
         {
            case Int i:
               return Int.Object(-i.Value).Matched();
            case Float f:
               return Float.Object(-f.Value).Matched();
            default:
               return $"{x.ClassName} can't be negated".FailedMatch<IObject>();
         }
      }

      public override string ToString() => "negate";
   }
}