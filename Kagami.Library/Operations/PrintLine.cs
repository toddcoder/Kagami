using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

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