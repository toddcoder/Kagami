using System.Text.RegularExpressions;
using Core.Matching;
using Kagami.Library.Nodes.Statements;
using Core.Monads;
using Core.Monads.Lazy;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class DeferParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(defer)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      Block block;
      if (state.CurrentSource.IsMatch("/s* '{'"))
      {
         var _block = getBlock(state);
         if (_block)
         {
            block = _block;
         }
         else
         {
            return _block.Exception;
         }
      }
      else
      {
         return fail("Defer requires a block structure");
      }

      block.AddReturnIf();
      state.AddStatement(new Defer(block));

      return unit;
   }
}