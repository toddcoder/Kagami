using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class PopFrame : Operation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         return machine.PopFrame().FlatMap(f => notMatched<IObject>(), failedMatch<IObject>);
      }

      public override string ToString() => "pop.frame";
   }
}