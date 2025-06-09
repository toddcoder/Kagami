using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Inclusions;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class InclusionParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(inclusion)(\s+)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var inclusionName = tokens[4].Text;
      var inclusion = new Inclusion(inclusionName);
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);

      var inheritedInclusionsParser = new InheritedInclusionsParser(inclusion);
      Optional<Unit> _result;
      while (state.More)
      {
         _result = inheritedInclusionsParser.Scan(state);
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

      _result = state.BeginBlock();
      if (!_result)
      {
         return _result.Exception;
      }

      var inclusionMembersParser = new InclusionMembersParser(inclusion);
      while (state.More)
      {
         _result = inclusionMembersParser.Scan(state);
         if (_result)
         {
         }
         else if (_result.Exception is (true, var exception2))
         {
            return exception2;
         }
         else
         {
            break;
         }
      }

      _result = state.EndBlock();
      if (_result)
      {
         Module.Global.Value.RegisterInclusion(inclusion);
         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}