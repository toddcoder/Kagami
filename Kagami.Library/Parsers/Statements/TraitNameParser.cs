using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Standard.Types.Collections;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class TraitNameParser : Parser
   {
      Hash<string, TraitClass> traits;

      public TraitNameParser(Hash<string, TraitClass> traits) : base(true) => this.traits = traits;

      public override string Pattern => $"^ /(/s*) /({ParserFunctions.REGEX_CLASS}) (/(/s*) /',')?";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens)
      {
         var traitName = tokens[2].Text;
         var more = tokens[4].Text == ",";
         state.Colorize(tokens, Color.Whitespace, Color.Class, Color.Whitespace, Color.Structure);

         if (Module.Global.Trait(traitName).If(out var trait))
         {
            if (!traits.ContainsKey(traitName))
               traits[traitName] = trait;
            More = more;
            return Unit.Matched();
         }
         else
            return failedMatch<Unit>(traitNotFound(traitName));
      }

      public bool More { get; set; }
   }
}