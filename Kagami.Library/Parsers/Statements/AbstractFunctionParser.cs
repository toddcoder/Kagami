using System.Text.RegularExpressions;
using Core.Collections;
using Core.Monads;
using Core.Strings;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class AbstractFunctionParser(Set<AbstractFunction> abstractFunctions) : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(abstract)(\s+)(func)(\s+)({REGEX_INVOKABLE})(\()?")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var functionName = tokens[6].Text;
      var isGetter = tokens[7].Text.IsEmpty();
      var isSetter = functionName.EndsWith('=');
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Invokable,
         Color.OpenParenthesis);

      var _result =
         from parametersValue in getParameters(state)
         from possibleTypeConstraint in parseTypeConstraint(state)
         select (parametersValue, possibleTypeConstraint);
      if (_result is (true, var (parameters, typeConstraint)))
      {
         Selector selector;
         if (isGetter)
         {
            selector = functionName.get();
         }
         else if (isSetter)
         {
            selector = functionName.set();
         }
         else
         {
            selector = parameters.Selector(functionName);
         }

         var abstractFunction = new AbstractFunction(selector, parameters, typeConstraint.Maybe);
         abstractFunctions.Add(abstractFunction);
      }

      return unit;
   }
}