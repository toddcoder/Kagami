using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;

namespace Kagami.Library.Parsers.Statements
{
   public class PutParser : EndingInExpressionParser
   {
      public override string Pattern => "^ /'put' /(/s+)";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword, Color.Whitespace);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         state.AddStatement(new Put(expression));
         return Unit.Matched();
      }
   }
}