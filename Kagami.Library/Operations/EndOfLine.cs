using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class EndOfLine : Operation
   {
      public override IMatched<IObject> Execute(Machine machine) => notMatched<IObject>();

      public override string ToString() => "end.of.line";
   }
}