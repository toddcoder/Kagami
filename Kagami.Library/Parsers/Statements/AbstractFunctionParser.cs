using System.Text.RegularExpressions;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Kagami.Library.Parsers.Statements.FunctionParser;

namespace Kagami.Library.Parsers.Statements;

public partial class AbstractFunctionParser : StatementParser
{
   [GeneratedRegex($@"^(\s*){REGEX_HIDDEN}(abstract)(\s+)(func)(\s+)({REGEX_FUNCTION_NAME})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var isHidden = tokens[2].Text.IsNotEmpty();
      var functionName = tokens[7].Text;
      var needsParameters = tokens[8].Text == "(";
      if (needsParameters)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Invokable,
            Color.OpenParenthesis);
      }
      else
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Invokable);
         functionName = $"__${functionName}";
      }

      var _parameters = GetAnyParameters(needsParameters, state);
      if (_parameters is (true, var parameters))
      {
         var _response = parseTypeConstraint(state);
         if (_response is (true, var possibleTypeConstraint))
         {
            var _typeConstraint = possibleTypeConstraint.Maybe;
            var block = new Block(new AbstractFail(functionName), _typeConstraint);
            var function = new Function(functionName, parameters, isHidden, block, false, false, "");
            state.AddStatement(function);

            return unit;
         }
         else
         {
            return _response.Exception;
         }
      }
      else
      {
         return _parameters.Exception;
      }
   }
}