using Standard.Types.Maybe;

namespace Kagami.Library.Nodes.Symbols
{
   public class Comparisand
   {
      public Comparisand(Expression expression, IMaybe<WhenSymbol> when)
      {
         expression.MakeComparisand();
         Expression = expression;
         When = when;
      }

      public Expression Expression { get; }

      public IMaybe<WhenSymbol> When { get; }

      public override string ToString() => $"{Expression}{When.FlatMap(w => w.ToString(), () => "")}";
   }
}