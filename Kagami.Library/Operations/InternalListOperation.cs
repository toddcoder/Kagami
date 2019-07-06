using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public abstract class InternalListOperation : OneOperandOperation
   {
      public abstract IMatched<IObject> Execute(Machine machine, Container list);

      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         if (value is Container list)
         {
	         return Execute(machine, list);
         }
         else
         {
	         return failedMatch<IObject>(incompatibleClasses(value, "InternalList"));
         }
      }
   }
}