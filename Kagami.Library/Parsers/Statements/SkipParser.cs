using Kagami.Library.Nodes.Statements;
using Standard.Types.Monads;

namespace Kagami.Library.Parsers.Statements
{
   public class SkipParser : StatementParser
   {
      public override string Pattern => "^ /'skip' /b";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword);
         state.AddStatement(new Skip());

         return Unit.Matched();
      }
   }
}