using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class OpenEnumParser : StatementParser
{
   [GeneratedRegex($@"^(open)(\s+)({REGEX_CLASS})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[3].Text;
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class);
      state.AddStatement(new OpenEnum(className));

      return unit;
   }
}