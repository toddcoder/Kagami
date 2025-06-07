using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Inclusions;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class InheritedInclusionsParser(Inclusion inclusion) : StatementParser
{
   [GeneratedRegex(@"^(\s*)(:)(\s+)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var inclusionName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Structure, Color.Whitespace, Color.Class);

      var _inheritedInclusion = Module.Global.Value.Inclusion(inclusionName);
      if (_inheritedInclusion is (true, var inheritedInclusion))
      {
         inclusion.Register(inheritedInclusion);
      }
      else
      {
         return inclusionNotFound(inclusionName);
      }

      while (state.More)
      {
         var _nextInclusionName =
            from prefix in state.Scan(@"^(\s*)(,)(\s*)", Color.Whitespace, Color.Structure, Color.Whitespace)
            from name in state.Scan(@$"^({REGEX_CLASS})\b", Color.Class)
            select name;
         if (_nextInclusionName is (true, var nextInclusionName))
         {
            _inheritedInclusion = Module.Global.Value.Inclusion(nextInclusionName);
            if (_inheritedInclusion is (true, var inheritedInclusion2))
            {
               inclusion.Register(inheritedInclusion2);
            }
            else
            {
               return inclusionNotFound(nextInclusionName);
            }
         }
         else if (_nextInclusionName.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            break;
         }
      }

      return unit;
   }
}