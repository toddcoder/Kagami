using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Inclusions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class RequiredOrOptionalFunctionParser(Inclusion inclusion) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(required|optional)(\s+)({REGEX_SELECTOR}){REGEX_ANTICIPATE_END}")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      try
      {
         var type = tokens[2].Text;
         var selectorSource = tokens[4].Text;
         var getterSetter = tokens[5].Text;

         if 
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Selector, Color.Selector);

         Result<Unit> _result;
         if (type == "required")
         {
            _result = inclusion.Register(new RequiredFunction(selector));
         }
         else
         {
            _result = inclusion.Register(new OptionalFunction(selector));
         }

         if (!_result)
         {
            return _result.Exception;
         }

         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}