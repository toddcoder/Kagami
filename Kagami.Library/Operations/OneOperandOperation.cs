using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public abstract class OneOperandOperation : Operation
   {
      public abstract IMatched<IObject> Execute(Machine machine, IObject value);

      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.Pop().If(out var value, out var exception))
         {
	         return Execute(machine, value);
         }
         else
         {
	         return failedMatch<IObject>(exception);
         }
      }
   }
}