using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class AssignOperatorParser(ExpressionBuilder builder) : SymbolParser(builder)
{
   [GeneratedRegex($@"^(\s*)({REGEX_FIELD})(\s*)(:=)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var fieldName = tokens[2].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Operator);

      var _expression = getExpression(state, ExpressionFlags.Standard);
      if (_expression is (true, var expression))
      {
         builder.Add(new AssignOperator(fieldName, expression));
         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}