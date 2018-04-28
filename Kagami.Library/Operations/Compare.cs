using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class Compare : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         if (x is IObjectCompare cx)
            return Int.Object(cx.Compare(y)).Matched();
         else
            return failedMatch<IObject>(incompatibleClasses(y, x.ClassName));
      }

      public override string ToString() => "compare";
   }
}