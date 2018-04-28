using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class NewOpenRange : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         if (y is Lambda lambda)
            return new OpenRange(x, lambda).Matched<IObject>();
         else
            return failedMatch<IObject>(incompatibleClasses(y, "Lambda"));
      }

      public override string ToString() => "new.open.range";
   }
}