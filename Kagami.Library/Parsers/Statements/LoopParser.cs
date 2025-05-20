using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public class LoopParser : StatementParser
{
   public override string Pattern => $"^ /'loop' {REGEX_ANTICIPATE_END}";

   protected static Optional<Expression> getUntil(ParseState state)
   {
      var untilParser = new UntilParser();
      return untilParser.Scan(state).Map(_ => untilParser.Expression);
   }

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      state.Colorize(tokens, Color.Keyword);

      var _result =
         from skipped in state.SkipEndOfLine()
         from b in getBlock(state)
         from e in getUntil(state)
         select (b, e);

      if (_result is (true, var (block, expression)))
      {
         state.AddStatement(new Loop(block, expression));
         state.CommitTransaction();

         return unit;
      }
      else
      {
         state.RollBackTransaction();
         return _result.Exception;
      }
   }
}