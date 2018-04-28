using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

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