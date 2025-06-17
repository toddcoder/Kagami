using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public partial class LoopParser : StatementParser
{
   [GeneratedRegex($@"^(\s*)(loop)\b")]
   public override partial Regex Regex();

   protected static Optional<(bool, Expression)> getUntil(ParseState state)
   {
      var untilParser = new LoopControlParser();
      return untilParser.Scan(state).Map(_ => (untilParser.IsUntil, untilParser.Expression));
   }

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.BeginTransaction();

      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      var _result =
         from b in getBlock(state)
         from e in getUntil(state)
         select (b, e);

      if (_result is (true, var (block, (isUntil, expression))))
      {
         state.AddStatement(new Loop(block, expression, isUntil));
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