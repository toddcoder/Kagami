using Kagami.Library.Nodes.Statements;
using Standard.Types.Monads;

namespace Kagami.Library.Parsers.Statements
{
   public class PrintLnParser : StatementParser
   {
      public override string Pattern => "^ /'println'";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword);
         state.AddStatement(new Print());

         return Unit.Matched();
      }
   }
}