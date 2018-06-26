using Kagami.Library.Classes;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class TraitImplementsParser : StatementParser
   {
      Hash<string, TraitClass> traits;

      public TraitImplementsParser(Hash<string, TraitClass> traits) => this.traits = traits;

      public override string Pattern => "^ /'implements' /b";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword);

         while (state.More)
         {
            var parser = new TraitNameParser(traits);
            if (parser.Scan(state).If(out var _, out var isNotMatched, out var exception))
            {
               if (!parser.More)
                  break;
            }
            else if (isNotMatched)
               break;
            else
               return failedMatch<Unit>(exception);
         }

         return Unit.Matched();

      }
   }
}