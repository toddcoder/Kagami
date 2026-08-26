using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class BuilderDoParser(BuilderState builderState) : StatementParser
{
   [GeneratedRegex(@"^(\s*)(do)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _block = getAnyBlock(state);
      if (_block is (true, var block))
      {
         state.AddStatement(new BuilderDo(builderState, block));
         return unit;
      }
      else
      {
         return _block.Exception;
      }
   }
}