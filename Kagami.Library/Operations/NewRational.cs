using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class NewRational : TwoNumericOperation
   {
      public override IMatched<IObject> Execute(Machine machine, INumeric x, INumeric y)
      {
         return ((Rational)(x.AsBigInteger(), y.AsBigInteger())).Matched<IObject>();
      }

      public override string ToString() => "new.rational";
   }
}