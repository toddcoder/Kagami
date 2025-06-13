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
      ReturnValue returnValue;
      if (returnTopOfStack)
      {
         var _value = Machine.Current.Value.CurrentFrame.Pop().Optional();
         if (_value is (true, var value))
         {
            returnValue = new ReturnValue.Value(value);
         }
         else
         {
            returnValue = new ReturnValue.EmptyStack();
         }
      }
      else
      {
         returnValue = new ReturnValue.NoValue();
      }

      var frames = machine.PopFrames();
      if (frames.FunctionFrame is (true, var frame))
      {
         if (frame.Address is (true, var address))
         {
            machine.GoTo(address);
         }

         return returnValue switch
         {
            ReturnValue.EmptyStack => emptyStack("return"),
            ReturnValue.NoValue => nil,
            ReturnValue.Value value => copyFields(value.Object, frames).Just(),
            _ => new ArgumentOutOfRangeException(nameof(returnValue))
         };
      }
      else
      {
         return fail("Function frame not pushed");
      }
   }

   protected bool returnTopOfStack;

   public Return(bool returnTopOfStack) => this.returnTopOfStack = returnTopOfStack;

   public override Optional<IObject> Execute(Machine machine) => ReturnAction(machine, returnTopOfStack);

   public override bool Increment => true;

   public override string ToString() => $"return({returnTopOfStack.ToString().ToLower()})";

   public bool ReturnTopOfStack => returnTopOfStack;
}