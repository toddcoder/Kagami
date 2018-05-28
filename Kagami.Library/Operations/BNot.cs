using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class BNot : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         if (value is Int i)
            return Int.IntObject(~i.Value).Matched();
         else
            return failedMatch<IObject>(incompatibleClasses(value, "Int"));
      }

      public override string ToString() => "bnot";
   }
}