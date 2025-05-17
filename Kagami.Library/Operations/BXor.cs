using Core.Monads;

namespace Kagami.Library.Operations;

public class BXor : TwoIntOperation
{
   public override Optional<int> Execute(int x, int y) => x ^ y;

   public override string ToString() => "bxor";
}