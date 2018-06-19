using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class TraitParser : StatementParser
   {
      public override string Pattern => $"^ /'trait' /(|s+|) /({REGEX_CLASS})";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var traitName = tokens[3].Text;
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class);

         var traitClass = new TraitClass(traitName);
         if (Module.Global.RegisterTrait(traitClass).If(out _, out var exception))
         {
            var traitItemsParser = new TraitItemsParser(traitClass);
            if (state.Advance().If(out _, out var original))
            {
               while (state.More)
                  if (traitItemsParser.Scan(state).If(out _, out var isNotMatched, out exception)) { }
                  else if (isNotMatched)
                     break;
                  else
                     return failedMatch<Unit>(exception);

               state.Regress();
               return Unit.Matched();
            }
            else
               return original;
         }
         else
            return failedMatch<Unit>(exception);
      }
   }
}