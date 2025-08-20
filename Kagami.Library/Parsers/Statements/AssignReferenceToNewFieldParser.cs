using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class AssignReferenceToNewFieldParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(var)(\s+)({REGEX_FIELD})(\s*)(=)(\s+)(ref)(\s+)({REGEX_FIELD})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var targetField = tokens[4].Text;
      var sourceField = tokens[10].Text;

      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure, Color.Whitespace,
         Color.Keyword, Color.Whitespace, Color.Identifier);

      state.AddStatement(new AssignReferenceToNewField(sourceField, targetField));
      return unit;
   }
}