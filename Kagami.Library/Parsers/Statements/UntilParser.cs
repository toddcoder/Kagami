using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;

namespace Kagami.Library.Parsers.Statements
{
   public class UntilParser : EndingInExpressionParser
   {
      public override string Pattern => "^ /'until' /b";

      public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword);
         return Unit.Matched();
      }

      public override IMatched<Unit> Suffix(ParseState state, Expression expression)
      {
         Expression = expression;
         return Unit.Matched();
      }

      public Expression Expression { get; set; }
   }
}