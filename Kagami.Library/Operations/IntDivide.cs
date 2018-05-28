using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class IntDivide : TwoNumericOperation
   {
      public override IMatched<IObject> Execute(Machine machine, INumeric x, INumeric y)
      {
         var ix = x.AsInt32();
         var iy = y.AsInt32();

         return Int.IntObject(ix / iy).Matched();
      }

      public override string ToString() => "int.divide";
   }
}