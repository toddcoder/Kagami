using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
   public class Or : TwoBooleanOperation
   {
      public override IMatched<bool> Execute(Machine machine, bool x, bool y) => (x || y).Matched();

      public override string ToString() => "or";
   }
}