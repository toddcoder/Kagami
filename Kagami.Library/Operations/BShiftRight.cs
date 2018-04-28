using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class BShiftRight : TwoIntOperation
   {
      public override IMatched<int> Execute(Machine machine, int x, int y) => (x >> y).Matched();

      public override string ToString() => "bsr";
   }
}