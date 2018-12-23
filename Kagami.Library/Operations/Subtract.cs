using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Operations
{
   public class Subtract : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         return apply(x, y, (a, b) => a - b, (a, b) => a - b, (a, b) => a - b, (a, b) => a.Subtract(b), "-").Matched();
      }

      public override string ToString() => "subtract";
   }
}