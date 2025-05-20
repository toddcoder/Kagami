using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public class ReturnNothingParser : StatementParser
{
   public override string Pattern => $"^ /'return' /({REGEX_EOL})";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Keyword, Color.Whitespace);
      state.AddStatement(new ReturnNothing());

      return unit;
   }
}