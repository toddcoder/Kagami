using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
   public class Swap : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         machine.Push(y);
         return x.Matched();
      }

      public override string ToString() => "swap";
   }
}