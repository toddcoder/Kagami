using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class DoesParser(ClassBuilder builder) : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(does)(\s+)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var protocolName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);
      builder.AddProtocol(protocolName);

      return unit;
   }
}