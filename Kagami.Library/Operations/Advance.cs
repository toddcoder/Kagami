using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class Advance : Operation
   {
      int increment;

      public Advance(int increment) => this.increment = increment;

      public override IMatched<IObject> Execute(Machine machine)
      {
         machine.Advance(increment);
         return notMatched<IObject>();
      }

      public override bool Increment => false;

      public override string ToString() => $"advance({increment})";
   }
}