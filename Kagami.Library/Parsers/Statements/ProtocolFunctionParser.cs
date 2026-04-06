using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class ProtocolFunctionParser(ProtocolBuilder builder) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(fn)(\s+)({REGEX_SELECTOR})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      try
      {
         Selector selector = tokens[4].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Selector);

         builder.AddSelector(selector);

         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}