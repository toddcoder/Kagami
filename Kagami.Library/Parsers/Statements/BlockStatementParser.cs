using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class BlockStatementParser : StatementParser
   {
      public override string Pattern => $"^ /'block' /({REGEX_EOL})";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword, Color.Whitespace);
         if (getBlock(state).ValueOrCast<Unit>(out var block, out var asUnit))
         {
            state.AddStatement(new BlockStatement(block));
            return Unit.Matched();
         }
         else
         {
            return asUnit;
         }
      }
   }
}