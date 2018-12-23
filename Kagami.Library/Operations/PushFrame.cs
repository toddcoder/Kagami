using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class PushFrame : Operation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         machine.PushFrame(new Frame());
         return notMatched<IObject>();
      }

      public override string ToString() => "push.frame";
   }
}