using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class DefParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(def)(\s+)({REGEX_FIELD})(\s*)(=)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier, Color.Whitespace, Color.Structure);

      var _expression = getExpression(state, ExpressionFlags.InLambda);
      if (_expression is (true, var expression))
      {
         var lambdaSymbol = new LambdaSymbol(0, expression);
         var assignDefinition = new AssignDefinition(fieldName, lambdaSymbol);
         state.AddStatement(assignDefinition);

         return unit;
      }
      else
      {
         return _expression.Exception;
      }
   }
}