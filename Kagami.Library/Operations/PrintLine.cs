using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class PrintLine : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         machine.Context.PrintLine(stringOf(value));
         return notMatched<IObject>();
      }

      public override string ToString() => "println";
   }
}