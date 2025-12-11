using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Inclusions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class RequiredFunctionParser(Inclusion inclusion) : StatementParser
{
   [GeneratedRegex($@"^(\s*)(required)(\s+)(func)(\s+)({REGEX_SELECTOR})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      try
      {
         Selector selector = tokens[6].Text;

         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Selector);

         var _possibleTypeConstraint = parseTypeConstraint(state);
         Maybe<TypeConstraint> _typeConstraint = _possibleTypeConstraint.Map(ptc => ptc.Maybe);

         var _result = inclusion.Register(new RequiredFunction(selector, _typeConstraint, inclusion));

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