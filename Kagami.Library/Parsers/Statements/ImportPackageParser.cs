using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ImportPackageParser : StatementParser
{
   //public override string Pattern => $"^ /'import' /(/s+) /({REGEX_FIELD}) {REGEX_ANTICIPATE_END}";

   [GeneratedRegex($@"^(import)(\s+)({REGEX_FIELD}){REGEX_ANTICIPATE_END}")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var fieldName = tokens[3].Text;
      state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier);
      state.AddStatement(new ImportPackage(fieldName));

      return unit;
   }
}