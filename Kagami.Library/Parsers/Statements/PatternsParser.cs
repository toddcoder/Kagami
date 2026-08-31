using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class PatternsParser : StatementParser
{
   [GeneratedRegex($@"^(\s*)(patterns)(\()")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.OpenParenthesis);

      var _parameters = getParameters(state);
      if (_parameters is (true, var parameters))
      {
         var _scan = state.BeginBlock();
         if (_scan)
         {
            while (state.More)
            {
               _scan = state.EndBlock();
               if (_scan)
               {
                  break;
               }
               else if (_scan.Exception is (true, var exception))
               {
                  return exception;
               }

               var patternsMemberParser = new PatternsMemberParser(parameters);
               _scan = patternsMemberParser.Scan(state);
               if (!_scan)
               {
                  return _scan.Exception;
               }
            }
         }
         else
         {
            return _scan.Exception;
         }
      }

      return unit;
   }
}