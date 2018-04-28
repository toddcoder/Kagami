using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class Show : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => notMatched<IObject>();

      public override string ToString() => "show";
   }
}