using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class Break : Operation
   {
      public override IMatched<IObject> Execute(Machine machine) => notMatched<IObject>();

      public override string ToString() => "break";
   }
}