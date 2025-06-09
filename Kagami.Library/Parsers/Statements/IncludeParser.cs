using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class IncludeParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(include)(\s+)({REGEX_CLASS})\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var inclusionName = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);

      var _inclusion = Module.Global.Value.Inclusion(inclusionName);
      if (_inclusion is (true, var inclusion))
      {
         foreach (var requiredFunction in inclusion.RequiredFunctions())
         {
            state.AddStatement(requiredFunction);
         }

         foreach (var optionalFunction in inclusion.OptionalFunctions())
         {
            state.AddStatement(optionalFunction);
         }

         foreach (var function in inclusion.Functions())
         {
            state.AddStatement(function);
         }
      }
      else
      {
         return inclusionNotFound(inclusionName);
      }
      return unit;
   }
}