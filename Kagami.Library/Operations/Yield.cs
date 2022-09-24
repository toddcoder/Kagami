using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations
{
   public class Yield : Operation
   {
      public static Responding<IObject> YieldAction(Machine machine)
      {
         var topFrame = machine.CurrentFrame;
         var frames = machine.PopFrames();
         Responding<IObject> _value;
         if (topFrame.IsEmpty)
         {
            _value = None.NoneValue.Response();
         }
         else if (topFrame.Pop().Map(out var popped, out var exception))
         {
	         if (popped is None)
	         {
		         _value = popped.Response();
	         }
	         else
	         {
		         IObject copy;
		         if (popped is IPristineCopy pc)
		         {
			         copy = pc.Copy();
		         }
		         else
		         {
			         copy = popped;
		         }

		         if (copy is ICopyFields cf)
		         {
			         cf.CopyFields(frames.Fields);
		         }

		         popped = copy;
		         _value = new YieldReturn(popped, machine.Address, frames).Response<IObject>();
	         }
         }
         else
         {
            return exception;
         }

         if (frames.FunctionFrame.Map(out var frame))
         {
            var returnAddress = frame.Address;

            if (machine.GoTo(returnAddress))
            {
	            return _value;
            }
            else
            {
               return badAddress(returnAddress);
            }
         }
         else
         {
            return invalidStack();
         }
      }

      public override Responding<IObject> Execute(Machine machine) => YieldAction(machine);

      public override string ToString() => "yield";

      public override bool Increment => false;
   }
}