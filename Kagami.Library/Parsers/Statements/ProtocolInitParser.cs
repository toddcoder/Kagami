using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class ProtocolInitParser(ProtocolBuilder builder) : StatementParser
{
    [GeneratedRegex(@"^(\s*)(init)(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.OpenParenthesis);

      var _parameters = getParameters(state);
      if (_parameters is (true, var parameters))
      {
         var selector = parameters.Selector("init");
         builder.AddSelector(selector);

         return unit;
      }
      else
      {
         return _parameters.Exception;
      }
    }
}