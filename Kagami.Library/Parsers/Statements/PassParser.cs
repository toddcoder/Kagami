using Kagami.Library.Nodes.Statements;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements;

public class PassParser : StatementParser
{
   public override string Pattern => "^ /('pass' | 'endseq') /b";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Keyword);
      state.AddStatement(new Pass());

      return unit;
   }
}