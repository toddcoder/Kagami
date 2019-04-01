using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class BShiftLeft : TwoIntOperation
   {
      public override IMatched<int> Execute(Machine machine, int x, int y) => (x << y).Matched();

      public override string ToString() => "bsl";
   }
}