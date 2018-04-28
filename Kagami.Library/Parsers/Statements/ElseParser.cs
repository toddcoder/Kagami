using Kagami.Library.Nodes.Statements;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ElseParser : StatementParser
   {
      public override string Pattern => $"^ /'else' /({REGEX_EOL})";

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         state.Colorize(tokens, Color.Keyword, Color.Whitespace);
         if (getBlock(state).If(out var block, out var original))
         {
            Block = block.Some();
            return Unit.Matched();
         }
         else
            return original.Unmatched<Unit>();
      }

      public IMaybe<Block> Block { get; set; } = none<Block>();
   }
}