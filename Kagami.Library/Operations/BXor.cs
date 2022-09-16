using Core.Monads;

namespace Kagami.Library.Operations
{
   public class BXor : TwoIntOperation
   {
      public override IMatched<int> Execute(int x, int y) => (x ^ y).Matched();

      public override string ToString() => "bxor";
   }
}