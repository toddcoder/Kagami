using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Stop : Operation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         machine.Running = false;
         return notMatched<IObject>();
      }

      public override string ToString() => "stop";
   }
}