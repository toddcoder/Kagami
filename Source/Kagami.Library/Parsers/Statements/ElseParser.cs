using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ElseParser : StatementParser
   {
      public override string Pattern => $"^ /'else' /({REGEX_EOL})";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword, Color.Whitespace);
         if (getBlock(state).ValueOrCast<Unit>(out var block, out var asUnit))
         {
            Block = block.Some();
            return Unit.Matched();
         }
         else
         {
            return asUnit;
         }
      }

      public IMaybe<Block> Block { get; set; } = none<Block>();
   }
}