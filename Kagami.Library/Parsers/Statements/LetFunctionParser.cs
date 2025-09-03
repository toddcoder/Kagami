using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class LetFunctionParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(let)(\s+)({REGEX_FUNCTION_NAME})(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var fieldName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Invokable, Color.OpenParenthesis);
      var _result =
         from parametersValue in getParameters(state)
         from equal in state.Scan(@"^(\s*)(=)", Color.Whitespace, Color.Structure)
         from possibleTypConstraintValue in parseTypeConstraint(state)
         from blockValue in getSingleLine(state, possibleTypConstraintValue.Maybe)
         select (parametersValue, blockValue);
      if (_result is (true, var (parameters, block)))
      {
         var lambdaSymbol = new LambdaSymbol(parameters, block);
         var function = new AssignLambda(fieldName, lambdaSymbol);
         state.AddStatement(function);

         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}