using Kagami.Library.Nodes.Statements;
using Core.Monads;

namespace Kagami.Library.Parsers.Statements
{
   public class ExitParser : StatementParser
   {
      public override string Pattern => "^ /'exit' /b";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword);
         state.AddStatement(new Exit());

         return Unit.Matched();
      }
   }
}