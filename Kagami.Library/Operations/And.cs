using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class And : TwoBooleanOperation
   {
      public override IMatched<bool> Execute(Machine machine, bool x, bool y) => (x && y).Matched();

      public override string ToString() => "and";
   }
}