using Kagami.Library.Classes;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class IncludeTraitParser : StatementParser
   {
      TraitClass targetTraitClass;

      public IncludeTraitParser(TraitClass targetTraitClass) => this.targetTraitClass = targetTraitClass;

      public override string Pattern => $"^ /'include' /(|s+|) /({REGEX_CLASS}) /b";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var sourceTraitName = tokens[3].Text;
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class);

         if (Module.Global.Trait(sourceTraitName).If(out var sourceTraitClass))
            if (targetTraitClass.CopyFrom(sourceTraitClass).If(out _, out var exception))
               return Unit.Matched();
            else
               return failedMatch<Unit>(exception);
         else
            return failedMatch<Unit>(traitNotFound(sourceTraitName));
      }
   }
}