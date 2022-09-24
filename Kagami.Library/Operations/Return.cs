using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Operations.OperationFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Return : Operation
   {
      public static Responding<IObject> ReturnAction(Machine machine, bool returnTopOfStack)
      {
         var _response = Machine.Current.CurrentFrame.Pop().Responding();

         var frames = machine.PopFrames();
         if (frames.FunctionFrame.Map(out var frame))
         {
            var returnAddress = frame.Address;
            if (returnTopOfStack)
            {
               if (_response.Map(out var v, out var _exception))
               {
                  _response = copyFields(v, frames).Response();
               }
               else if (_exception.Map(out var exception))
               {
                  return exception;
               }
               else
               {
                  return emptyStack();
               }
            }
            else
            {
               _response = nil;
            }

            return machine.GoTo(returnAddress) ? _response : badAddress(returnAddress);
         }
         else
         {
            return invalidStack();
         }
      }

      protected bool returnTopOfStack;

      public Return(bool returnTopOfStack) => this.returnTopOfStack = returnTopOfStack;

      public override Responding<IObject> Execute(Machine machine) => ReturnAction(machine, returnTopOfStack);

      public override bool Increment => false;

      public override string ToString() => $"return({returnTopOfStack.ToString().ToLower()})";

      public bool ReturnTopOfStack => returnTopOfStack;
   }
}