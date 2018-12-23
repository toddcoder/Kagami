using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Peek : Operation
   {
      int index;

      public Peek(int index) => this.index = index;

      public override string ToString() => $"peek({index})";

      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.CurrentFrame.Peek().If(out var value))
         {
            var currentAddress = machine.Address;
            var message = $"{value.Image} | {value.ClassName}";
            if (currentAddress != machine.Address)
               machine.GoTo(currentAddress);
            machine.Context.Peek(message, index);
         }

         return notMatched<IObject>();
      }
   }
}