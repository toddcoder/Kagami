using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class Negate : OneOperandOperation
{
   public override string ToString() => "negate";

   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Int i => Int.IntObject(-i.Value).Just(),
      Float f => Float.FloatObject(-f.Value).Just(),
      Long l => Long.LongObject(-l.Value).Just(),
      Complex c => c.Negate().Just(),
      Rational r => r.Negate().Just(),
      _ => classOf(value).SendMessage(value, "negate()", []).Just()
   };
}