using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class PragmaParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(#pragma)(\s+)([A-Z_][A-Z_0-9]*)(\s+)(\S+)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var pragmaName = tokens[4].Text;
      var pragmaArgument = tokens[6].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Identifier);

      switch (pragmaName)
      {
         case "ALLOW_PRINT_STATEMENT":
         {
            state.AllowPrintStatement = pragmaArgument.Equals("true", StringComparison.CurrentCultureIgnoreCase);
            break;
         }
         default:
            return fail($"Didn't understand pragma {pragmaName}");
      }

      return unit;
   }
}