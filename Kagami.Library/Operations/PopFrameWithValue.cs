using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class PopFrameWithValue : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var _frame = machine.PopFrame();
      if (_frame is (true, var frame))
      {
         frame.ExecuteDeferred(machine);
         return frame.Pop().Optional();
      }
      else
      {
         return _frame.Exception;
      }
   }

   public override string ToString() => "pop.frame.with.value";
}