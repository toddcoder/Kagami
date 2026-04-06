using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class ProtocolGetterParser(ProtocolBuilder builder) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(get)(\s+)({REGEX_FIELD})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var propertyName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier);

      var getter = propertyName.get();
      builder.AddSelector(getter);

      return unit;
   }
}