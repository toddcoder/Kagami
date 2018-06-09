using Kagami.Library.Nodes.Statements;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ReturnNothingParser : StatementParser
   {
      public override string Pattern => $"^ /'return' /({REGEX_EOL})";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword, Color.Whitespace);
         state.AddStatement(new ReturnNothing());

         return Unit.Matched();
      }
   }
}