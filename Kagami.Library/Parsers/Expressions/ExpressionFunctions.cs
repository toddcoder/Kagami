using Kagami.Library.Nodes;
using Kagami.Library.Nodes.Symbols;

namespace Kagami.Library.Parsers.Expressions;

public static class ExpressionFunctions
{
   public static void evaluate(Symbol symbol, Func<Symbol, bool> predicate, Action<Symbol> action)
   {
      if (predicate(symbol))
      {
         action(symbol);
      }

      switch (symbol)
      {
         case Expression expression:
            foreach (var child in expression.Symbols)
            {
               evaluate(child, predicate, action);
            }

            break;
         case IHasExpression hasExpression:
            evaluate(hasExpression.Expression, predicate, action);
            break;
         case IHasExpressions hasExpressions:
            foreach (var expr in hasExpressions.Expressions)
            {
               evaluate(expr, predicate, action);
            }

            break;
         case IHasSymbol hasSymbol:
            evaluate(hasSymbol.Symbol, predicate, action);
            break;
      }
   }
}