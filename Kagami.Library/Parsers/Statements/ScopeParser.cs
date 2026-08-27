using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ScopeParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(but)(\s+)({REGEX_FIELD})(\s*)(=)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);

      var _expression = getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         state.AddStatement(new ScopeStatement(fieldName, expression));
         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}