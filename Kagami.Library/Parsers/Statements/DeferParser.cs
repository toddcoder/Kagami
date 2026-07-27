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
      LazyOptional<Statement> _statement = nil;
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
      else if (_statement.ValueOf(getStatement(state)) is (true, var statement))
      {
         block = [statement];
      }
      else
      {
         return _statement.Exception;
      }

      block.AddReturnIf();
      state.AddStatement(new Defer(block));

      return unit;
   }
}