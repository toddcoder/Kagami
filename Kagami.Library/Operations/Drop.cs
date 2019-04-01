using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Drop : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => notMatched<IObject>();

      public override string ToString() => "drop";
   }
}