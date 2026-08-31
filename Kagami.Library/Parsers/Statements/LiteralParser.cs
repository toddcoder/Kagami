using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class LiteralParser : StatementParser
{
   [GeneratedRegex($@"^(\s*)(literal)(\s+)({REGEX_FIELD})(\s*)(=)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var literalName = tokens[4].Text;
      return unit;
   }
}