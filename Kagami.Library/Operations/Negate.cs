using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class Negate : OneNumericOperation
   {
      public override IMatched<IObject> Execute(Machine machine, INumeric x) => Evaluate(x);

      public static IMatched<IObject> Evaluate(INumeric x)
      {
         switch (x)
         {
            case Int i:
               return Int.Object(-i.Value).Matched();
            case Float f:
               return Float.Object(-f.Value).Matched();
            case Long l:
               return Long.Object(-l.Value).Matched();
            case Complex c:
               return c.Negate().Matched();
            case Rational r:
               return r.Negate().Matched();
            default:
               return failedMatch<IObject>(notNumeric((IObject)x));
         }
      }

      public override string ToString() => "negate";
   }
}