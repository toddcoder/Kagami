using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public abstract class TwoNumericOperation : Operation
   {
      public abstract IMatched<IObject> Execute(Machine machine, INumeric x, INumeric y);

      public override IMatched<IObject> Execute(Machine machine)
      {
         try
         {
            if (machine.Pop().If(out var y, out var exception) && machine.Pop().If(out var x, out exception))
            {
	            if (x is INumeric nx)
	            {
		            if (y is INumeric ny)
		            {
			            return Execute(machine, nx, ny);
		            }
		            else
		            {
			            return failedMatch<IObject>(notNumeric(y.Image));
		            }
	            }
	            else
	            {
		            return failedMatch<IObject>(notNumeric(x.Image));
	            }
            }
            else
            {
	            return failedMatch<IObject>(exception);
            }
         }
         catch (Exception exception)
         {
            return failedMatch<IObject>(exception);
         }
      }
   }
}