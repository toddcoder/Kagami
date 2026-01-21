using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewCons : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      if (y is Objects.Cons cons)
      {
         return Objects.Cons.Combine(x, cons).Just();
      }
      else
      {
         return Objects.Cons.Cons1(x, y).Just();
      }
   }

   public override string ToString() => "new.cons";
}