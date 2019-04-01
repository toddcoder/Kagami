using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public abstract class OneNumericOperation : OneOperandOperation
   {
      public abstract IMatched<IObject> Execute(Machine machine, INumeric x);

      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         if (value is INumeric nx)
            return Execute(machine, nx);
         else
            return failedMatch<IObject>(incompatibleClasses(value, "Numeric"));
      }
   }
}