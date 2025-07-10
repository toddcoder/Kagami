using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class And : TwoOperandOperation
{
   public override string ToString() => "and";

   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y) => x switch
   {
      KBoolean b1 when y is KBoolean b2 => new KBoolean(b1.Value && b2.Value),
      Int i1 when y is Int i2 => new Int(i1.Value & i2.Value),
      Int => incompatibleClasses(y, "Int"),
      _ => incompatibleClasses(x, "Int")
   };
}