using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class SetX : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         machine.X = value;
         return notMatched<IObject>();
      }

      public override string ToString() => "set.x";
   }
}