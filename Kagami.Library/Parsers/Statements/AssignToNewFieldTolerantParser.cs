using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class AssignToNewFieldTolerantParser : EndingInExpressionParser
{
   protected string fieldName = "";

   [GeneratedRegex(@$"^(\s*)({REGEX_FIELD})(\s*)(:=)")]
   public override partial Regex Regex();

   public override Optional<Unit> Prefix(ParseState state, Token[] tokens)
   {
      fieldName = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);

      return unit;
   }

   public override Optional<Unit> Suffix(ParseState state, Expression expression)
   {
      state.AddStatement(new AssignToNewField(true, fieldName, true, expression));
      return unit;
   }
}