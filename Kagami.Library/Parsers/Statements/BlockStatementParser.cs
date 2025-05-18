using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public class BlockStatementParser : StatementParser
{
   public override string Pattern => $"^ /'block' /({REGEX_EOL})";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Keyword, Color.Whitespace);
      var _block = getBlock(state);
      if (_block is (true, var block))
      {
         state.AddStatement(new BlockStatement(block));
         return unit;
      }
      else
      {
         return _block.Exception;
      }
   }
}