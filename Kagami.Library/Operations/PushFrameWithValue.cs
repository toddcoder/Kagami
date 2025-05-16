using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class PushFrameWithValue : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      machine.PushFrame(new Frame());
      return value.Just();
   }

   public override string ToString() => "push.frame.with.value";
}