using Kagami.Library.Operations;
using Standard.Types.Maybe;

namespace Kagami.Library.Nodes.Symbols
{
   public class ComparisandSymbol : Symbol
   {
      Expression comparisand;
      IMaybe<WhenSymbol> when;

      public ComparisandSymbol(Comparisand comparisand)
      {
         this.comparisand = comparisand.Expression;
         when = comparisand.When;
      }

      public override void Generate(OperationsBuilder builder)
      {
         TODO_IMPLEMENT_ME();
      }

      public override Precedence Precedence { get; }
      public override Arity Arity { get; }
   }
}