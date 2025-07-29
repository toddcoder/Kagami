using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class DeclareNewFieldParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(var|let)(\s+)({REGEX_FIELD})(\s+)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var mutable = tokens[2].Text == "var";
      var fieldName = tokens[4].Text;
      var className = tokens[6].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Class);

      state.AddStatement(new DefineNewField(mutable, fieldName, className));
      return unit;
   }
}