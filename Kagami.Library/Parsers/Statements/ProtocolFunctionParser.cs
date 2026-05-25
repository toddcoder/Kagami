using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Invokables;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class ProtocolFunctionParser(ProtocolBuilder builder) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(fn)(\s+)({REGEX_FUNCTION_NAME})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      try
      {
         var functionName = tokens[4].Text;
         var hasParameters = tokens[5].Text == "(";
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Invokable, Color.OpenParenthesis);

         Parameters parameters;
         if (hasParameters)
         {
            var _parameters = getParameters(state);
            if (_parameters is (true, var newParameters))
            {
               parameters = newParameters;
            }
            else if (_parameters.Exception is (true, var exception))
            {
               return exception;
            }
            else
            {
               parameters = Parameters.Empty;
               functionName = functionName.get();
            }
         }
         else
         {
            parameters = Parameters.Empty;
            functionName = functionName.get();
         }

         var selector = parameters.Selector(functionName);
         builder.AddSelector(selector);

         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}