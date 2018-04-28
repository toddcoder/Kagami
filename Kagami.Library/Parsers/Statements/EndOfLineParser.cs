using Kagami.Library.Nodes.Statements;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Statements
{
   public class EndOfLineParser : StatementParser
   {
      public override string Pattern => "^ /(/r /n | /r | /n)";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Whitespace);
         state.AddStatement(new EndOfLine());

         return Unit.Matched();
      }

      public override bool IgnoreIndentation => true;
   }
}