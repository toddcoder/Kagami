using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
   public class PushFrameWithValue : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         machine.PushFrame(new Frame());
         return value.Matched();
      }

      public override string ToString() => "push.frame.with.value";
   }
}