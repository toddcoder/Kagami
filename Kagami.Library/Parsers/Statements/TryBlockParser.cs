using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class TryBlockParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(try)(\s*)({)")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Block);
      var _block = ParserFunctions.getPartialBlock(state, nil);
      if (_block is (true, var block))
      {
         state.AddStatement(new TryBlock(block));
         return unit;
      }
      else
      {
         return _block.Exception;
      }
   }
}