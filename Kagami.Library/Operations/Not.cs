using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
   public class Not : OneBooleanOperation
   {
      public override IMatched<bool> Execute(Machine machine, bool boolean) => (!boolean).Matched();

      public override string ToString() => "not";
   }
}