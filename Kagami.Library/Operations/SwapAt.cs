using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class SwapAt : Operation
{
   protected int index;

   public SwapAt(int index) => this.index = index;

   public override Optional<IObject> Execute(Machine machine)
   {
      var _swapped = machine.CurrentFrame.Swap(index);
      if (_swapped)
      {
         return nil;
      }
      else
      {
         return fail("Swap at out of range");
      }
   }

   public override string ToString() => $"swap.at({index})";
}