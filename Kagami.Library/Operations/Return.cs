using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Operations.OperationFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Return : Operation
{
   public static Optional<IObject> ReturnAction(Machine machine, bool returnTopOfStack)
   {
      var _response = Machine.Current.Value.CurrentFrame.Pop().Optional();

      var frames = machine.PopFrames();
      if (frames.FunctionFrame is (true, var frame))
      {
         //var returnAddress = frame.Address;
         if (returnTopOfStack)
         {
            if (_response is (true, var value))
            {
               _response = copyFields(value, frames).Just();
            }
            else if (_response.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               return emptyStack("value");
            }
         }
         else
         {
            _response = nil;
         }

         return _response; //machine.GoTo(returnAddress) ? _response : badAddress(returnAddress);
      }
      else
      {
         return invalidStack("frame");
      }
   }

   protected bool returnTopOfStack;

   public Return(bool returnTopOfStack) => this.returnTopOfStack = returnTopOfStack;

   public override Optional<IObject> Execute(Machine machine) => ReturnAction(machine, returnTopOfStack);

   public override bool Increment => true;

   public override string ToString() => $"return({returnTopOfStack.ToString().ToLower()})";

   public bool ReturnTopOfStack => returnTopOfStack;
}