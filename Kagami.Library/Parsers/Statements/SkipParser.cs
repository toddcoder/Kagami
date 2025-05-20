using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class SkipParser : StatementParser
{
   public override string Pattern => "^ /'skip' /b";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Keyword);
      state.AddStatement(new Skip());

      return unit;
   }
}