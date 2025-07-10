using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class Or : TwoOperandOperation
{
   public override string ToString() => "or";

   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y) => x switch
   {
      Int i1 when y is Int i2 => new Int(i1.Value | i2.Value),
      Int => incompatibleClasses(y, "Int"),
      _ => incompatibleClasses(x, "Int")
   };
}