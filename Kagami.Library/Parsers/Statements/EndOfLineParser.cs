using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class EndOfLineParser : StatementParser
{
   public override string Pattern => "^ /(/r/n | /r | /n | ';') /(/s*)";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Whitespace);
      state.AddStatement(new EndOfLine());

      return unit;
   }
}