using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Operations;

public class Multiply : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      return apply(x, y, (a, b) => a * b, (a, b) => a * b, (a, b) => a * b, (a, b) => a.Multiply(b), "*(_)").Just();
   }

   public override string ToString() => "multiply";
}