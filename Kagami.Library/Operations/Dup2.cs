using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class Dup2 : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      machine.Push(x);
      machine.Push(y);
      machine.Push(x);

      return y.Just();
   }

   public override string ToString() => "dup2";
}