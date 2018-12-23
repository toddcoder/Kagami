using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Drop : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => notMatched<IObject>();

      public override string ToString() => "drop";
   }
}