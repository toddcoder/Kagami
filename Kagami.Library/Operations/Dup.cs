using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Dup : Operation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         return machine.Peek().Map(v => v.Matched()).DefaultTo(() => failedMatch<IObject>(requiresNOperands(1)));
      }

      public override string ToString() => "dup";
   }
}