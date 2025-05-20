using Core.Monads;
using Kagami.Library.Nodes.Statements;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class StopParser : StatementParser
{
   public override string Pattern => "^ /'stop' /b";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Keyword);
      state.AddStatement(new Stop());

      return unit;
   }
}