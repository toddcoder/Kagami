using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Defer(Selector selector) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var _frame = machine.PeekFrame();
      if (_frame is (true, var frame))
      {
         frame.Defer(selector);
         return nil;
      }
      else
      {
         return fail("No frame to defer in");
      }
   }

   public override string ToString() => $"defer({selector})";
}