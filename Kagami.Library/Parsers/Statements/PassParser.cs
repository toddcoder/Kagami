using Kagami.Library.Nodes.Statements;
using Core.Monads;

namespace Kagami.Library.Parsers.Statements
{
   public class PassParser : StatementParser
   {
      public override string Pattern => "^ /'pass' /b";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword);
         state.AddStatement(new Pass());

         return Unit.Matched();
      }
   }
}