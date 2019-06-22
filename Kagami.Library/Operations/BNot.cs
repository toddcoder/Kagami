using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class BNot : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         if (value is Int i)
         {
	         return Int.IntObject(~i.Value).Matched();
         }
         else
         {
	         return failedMatch<IObject>(incompatibleClasses(value, "Int"));
         }
      }

      public override string ToString() => "bnot";
   }
}