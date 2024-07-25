using Standard.Types.Maybe;
using Standard.Types.RegularExpressions;
using static Kagami.Library.Parsers.ParserExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class IndentationParser : Parser
   {
      public override IMatched<Unit> Parse(ParseState state, Token[] tokens)
      {
         state.SkipEndOfLine();
         if (state.Scan(state.Indentation.FriendlyString(), Color.Whitespace).If(out _, out var original))
         {
            if (state.Scan("^ /(/s+)", Color.Whitespace).If(out var text, out original))
            {
               state.PushIndentation(text);
               return Unit.Matched();
            }

            if (original.IsNotMatched)
               return failedMatch<Unit>(badIndentation());

            return original.ExceptionAs<Unit>();
         }

         if (original.IsNotMatched)
            return failedMatch<Unit>(badIndentation());

         return original.ExceptionAs<Unit>();
      }

      public IndentationParser() : base(true) { }
   }
}