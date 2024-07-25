using Kagami.Library.Objects;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class NewRational : TwoNumericOperation
   {
      public override IMatched<IObject> Execute(INumeric x, INumeric y)
      {
         return ((Rational)(x.AsBigInteger(), y.AsBigInteger())).Matched<IObject>();
      }

      public override string ToString() => "new.rational";
   }
}