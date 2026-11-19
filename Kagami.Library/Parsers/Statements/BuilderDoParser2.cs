using Core.Monads;
using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Statements;

public partial class BuilderDoParser2(BuilderState builderState) : StatementParser
{
   [GeneratedRegex(@"^(\s*)(do)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _block = ParserFunctions.getSingleLine(state);
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