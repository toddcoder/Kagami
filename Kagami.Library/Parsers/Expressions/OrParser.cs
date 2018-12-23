using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;

namespace Kagami.Library.Parsers.Expressions
{
   public class OrParser : EndingInExpressionParser
   {
      public OrParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'or' /(/s+)";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         builder.Add(new OrSymbol(expression));
         return Unit.Matched();
      }
   }
}