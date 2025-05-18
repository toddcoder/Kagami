using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class Negate : OneNumericOperation
{
   public override Optional<IObject> Execute(Machine machine, INumeric x) => Evaluate(x);

   public static Optional<IObject> Evaluate(INumeric x) => x switch
   {
      Int i => Int.IntObject(-i.Value).Just(),
      Float f => Float.FloatObject(-f.Value).Just(),
      Long l => Long.LongObject(-l.Value).Just(),
      Complex c => c.Negate().Just(),
      Rational r => r.Negate().Just(),
      _ => notNumeric((IObject)x)
   };

   public override string ToString() => "negate";
}