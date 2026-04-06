using System.Text.RegularExpressions;
using Core.Applications;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class IncludeClassParser : StatementParser
{
   [GeneratedRegex(@$"^(\s*)(include\s+class)(\s+)({REGEX_CLASS})")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      var className = tokens[4].Text;
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Class);

      switch (className)
      {
         case "BaseError":
         {
            var source = getSource();
            var innerState = new ParseState(source);
            var classParser = new ClassParser();
            var _scan = classParser.Scan(innerState);
            if (_scan)
            {
               foreach (var statement in innerState.Statements())
               {
                  state.AddStatement(statement);
               }
            }
            else
            {
               return _scan.Exception;
            }

            return unit;
         }
         default:
            return fail($"Class {className} not understood");
      }

      string getSource()
      {
         var resources = new Resources<IncludeClassParser>();
         return resources.String($"{className}.kagami");
      }
   }
}