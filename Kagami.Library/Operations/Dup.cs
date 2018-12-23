using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Dup : Operation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         return machine.Peek().FlatMap(v => v.Matched(), () => failedMatch<IObject>(requiresNOperands(1)));
      }

      public override string ToString() => "dup";
   }
}