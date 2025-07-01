using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ReturnNothingParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(return){REGEX_ANTICIPATE_END}")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      state.AddStatement(new ReturnNothing());

      return unit;
   }
}