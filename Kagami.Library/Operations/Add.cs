using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Operations.NumericFunctions;

namespace Kagami.Library.Operations
{
   public class Add : TwoOperandOperation
   {
      public override Responding<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         return apply(x, y, (a, b) => a + b, (a, b) => a + b, (a, b) => a + b, (a, b) => a.Add(b), "+").Response();
      }

      public override string ToString() => "add";
   }
}