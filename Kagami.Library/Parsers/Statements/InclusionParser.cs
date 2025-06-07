using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Inclusions;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class InclusionParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(inclusion)(\s+)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var inclusionName = tokens[4].Text;
      var inclusion = new Inclusion(inclusionName);
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);

      var inheritedInclusionsParser = new InheritedInclusionsParser(inclusion);
      while (state.More)
      {
         var _result = inheritedInclusionsParser.Scan(state);
         if (_result)
         {
         }
         else if (_result.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      var _block = getBlock(state);
      if (!_block)
      {
         return _block.Exception;
      }



      return unit;
   }
}