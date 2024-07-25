using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;

namespace Kagami.Library.Parsers.Statements
{
   public class ReturnParser : EndingInExpressionParser
   {
      public override string Pattern => "^ /'return' /(|s+|)";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword, Color.Whitespace);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         state.AddStatement(new Return(expression, state.GetReturnType()));
         return Unit.Matched();
      }
   }
}