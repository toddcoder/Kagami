using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class TypeAssignmentParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(type)(\s+)({REGEX_CLASS})(\s*=\s*)({REGEX_CLASS})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var field = tokens[4].Text;
      var className = tokens[6].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class, Color.Structure, Color.Class);
      Module.Global.Value.ForwardReference(field);

      var expression = new Expression(new TypeConstraintSymbol([classOf(className)]));
      state.AddStatement(new AssignToNewField(false, field, expression, false, false));
      return unit;
   }
}