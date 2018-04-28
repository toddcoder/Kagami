using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Statements
{
   public class ReturnParser : EndingInExpressionParser
   {
      public override string Pattern => "^ /'return' /b";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         state.AddStatement(new Return(expression));
         return Unit.Matched();
      }
   }
}