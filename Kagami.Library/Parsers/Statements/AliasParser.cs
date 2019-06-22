using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class AliasParser : StatementParser
   {
      public override string Pattern =>
         $"^ /'alias' /(|s+|) /({REGEX_CLASS}) /(|s|) /'=' /(|s|) /({REGEX_CLASS}) {REGEX_ANTICIPATE_END}";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var aliasName = tokens[3].Text;
         var className = tokens[7].Text;
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Class, Color.Whitespace, Color.Structure, Color.Whitespace, Color.Class);

         if (Module.Global.Alias(aliasName, className).IfNot(out var exception))
         {
	         return failedMatch<Unit>(exception);
         }
         else
         {
	         return Unit.Matched();
         }
      }
   }
}