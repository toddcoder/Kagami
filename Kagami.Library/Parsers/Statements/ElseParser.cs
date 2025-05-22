using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class ElseParser : StatementParser
{
   public override string Pattern => "^ /(/s*) /'else' /b";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      var _block = getBlock(state);
      if (_block is (true, var block))
      {
         Block = block;
         return unit;
      }
      else
      {
         return _block.Exception;
      }
   }

   public Maybe<Block> Block { get; set; } = nil;
}