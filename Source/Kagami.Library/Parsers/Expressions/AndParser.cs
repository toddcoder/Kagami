using Kagami.Library.Nodes.Symbols;
using Core.Monads;

namespace Kagami.Library.Parsers.Expressions
{
   public class AndParser : EndingInExpressionParser
   {
      public AndParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'and' /(/s+)";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         builder.Add(new AndSymbol(expression));
         return Unit.Matched();
      }
   }
}