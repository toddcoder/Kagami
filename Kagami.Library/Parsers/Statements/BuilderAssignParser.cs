using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class BuilderAssignParser(BuilderState builderState, bool first) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(let)(\s+)({REGEX_FIELD})(\s*=)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Operator);

      var _expression = getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         state.AddStatement(new BuilderAssign(builderState, fieldName, expression, first));
      }
      else
      {
         return _expression.Exception;
      }

      return unit;
   }
}