using Kagami.Library.Objects;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class NewRational : TwoNumericOperation
   {
      public override Result<IObject> Execute(INumeric x, INumeric y)
      {
         return ((Rational)(x.AsBigInteger(), y.AsBigInteger())).Matched<IObject>();
      }

      public override string ToString() => "new.rational";
   }
}