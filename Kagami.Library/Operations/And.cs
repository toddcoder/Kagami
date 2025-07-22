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
      KChar c1 when y is KChar c2 => new KChar((char)(c1.Value & c2.Value)),
      KString s1 when y is KString s2 => new KChar((char)(s1[0].Value & s2[0].Value)),
      Int => incompatibleClasses(y, "Int"),
      _ => incompatibleClasses(x, "Int")
   };
}