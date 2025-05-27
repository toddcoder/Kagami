using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ReturnNothingParser : StatementParser
{
   //public override string Pattern => $"^ /'return' /({REGEX_EOL})";

   [GeneratedRegex($"^(return)({REGEX_EOL})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Keyword, Color.Whitespace);
      state.AddStatement(new ReturnNothing());

      return unit;
   }
}