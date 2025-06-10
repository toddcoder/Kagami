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
      var _value = Machine.Current.Value.CurrentFrame.Pop().Optional();
      if (_value is (true, var value))
      {
         var frames = machine.PopFrames();
         if (frames.FunctionFrame)
         {
            if (returnTopOfStack)
            {
               return copyFields(value, frames).Just();
            }
            else
            {
               return nil;
            }
         }
         else
         {
            return nil;
         }
      }
      else if (_value.Exception is (true, var exception))
      {
         return exception;
      }
      else if (returnTopOfStack)
      {
         return emptyStack("value");
      }

      return nil;
   }

   protected bool returnTopOfStack;

   public Return(bool returnTopOfStack) => this.returnTopOfStack = returnTopOfStack;

   public override Optional<IObject> Execute(Machine machine) => ReturnAction(machine, returnTopOfStack);

   public override bool Increment => true;

   public override string ToString() => $"return({returnTopOfStack.ToString().ToLower()})";

   public bool ReturnTopOfStack => returnTopOfStack;
}