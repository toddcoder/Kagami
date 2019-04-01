using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class GetX : Operation
   {
      public override IMatched<IObject> Execute(Machine machine) => machine.X.Matched();

      public override string ToString() => "get.x";
   }
}