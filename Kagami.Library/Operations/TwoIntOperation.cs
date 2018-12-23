using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public abstract class TwoIntOperation : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         if (x is Int xInt)
            if (y is Int yInt)
               return Execute(machine, xInt.Value, yInt.Value).Map(Int.IntObject);
            else
               return failedMatch<IObject>(incompatibleClasses(y, "Int"));
         else
            return failedMatch<IObject>(incompatibleClasses(x, "Int"));
      }

      public abstract IMatched<int> Execute(Machine machine, int x, int y);
   }
}