using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class Pipeline : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         if (y is Lambda lambda)
            return lambda.Invoke(x).Matched();
         else
            return failedMatch<IObject>(incompatibleClasses(y, "Lambda"));
      }

      public override string ToString() => "pipeline";
   }
}