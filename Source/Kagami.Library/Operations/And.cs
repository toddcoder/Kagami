using Core.Monads;

namespace Kagami.Library.Operations
{
   public class And : TwoBooleanOperation
   {
      public override IMatched<bool> Execute(bool x, bool y) => (x && y).Matched();

      public override string ToString() => "and";
   }
}