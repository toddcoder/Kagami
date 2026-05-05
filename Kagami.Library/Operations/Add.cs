using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Operations;

public class Add : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y) => Apply(x, y).Just();

   public static IObject Apply(IObject x, IObject y)
   {
      return apply(x, y, (a, b) => a + b, (a, b) => a + b, (a, b) => a + b, (a, b) => a.Add(b), "+(_)", (k, i) => k.Shift(i));
   }

   public override string ToString() => "add";
}