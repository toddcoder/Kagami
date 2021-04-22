using Core.Monads;

namespace Kagami.Library.Operations
{
   public class Or : TwoBooleanOperation
   {
      public override IMatched<bool> Execute(bool x, bool y) => (x || y).Matched();

      public override string ToString() => "or";
   }
}