using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Invokables;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class InitializeParser : StatementParser
{
   [GeneratedRegex(@"^(\s*)(init)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);
      state.CreateReturnType();
      var _block = getAnyBlock(state);
      if (_block is (true, var block))
      {
         state.RemoveReturnType();
         state.AddStatement(new Function("initialize", Parameters.Empty, false, block, false, false, ""));
      }
      else
      {
         state.RemoveReturnType();
         return _block.Exception;
      }

      return unit;
   }
}