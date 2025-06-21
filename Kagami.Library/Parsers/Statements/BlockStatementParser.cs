using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class BlockStatementParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(block)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
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