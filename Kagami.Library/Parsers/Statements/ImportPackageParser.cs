using Kagami.Library.Nodes.Statements;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ImportPackageParser : StatementParser
   {
      public override string Pattern => $"^ /'import' /(|s+|) /({REGEX_FIELD}) {REGEX_ANTICIPATE_END}";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var fieldName = tokens[3].Text;
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Identifier);
         state.AddStatement(new ImportPackage(fieldName));

         return Unit.Matched();
      }
   }
}