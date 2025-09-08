using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewCons : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y) => new Objects.Cons(x, y);

   public override string ToString() => "new.cons";
}