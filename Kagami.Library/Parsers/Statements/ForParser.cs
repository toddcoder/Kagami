using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements;

public class ForParser : StatementParser
{
   public override string Pattern => "^ /'for' /(/s+)";

   public override Optional<Unit> ParseStatement(ParseState state, Token[] tokens)
   {
      state.Colorize(tokens, Color.Keyword, Color.Whitespace);

      var _result =
         from comparisandValue in getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitColon)
         from scanned in state.Scan("^ /(/s*) /':='", Color.Whitespace, Color.Structure)
         from sourceValue in getExpression(state, ExpressionFlags.Standard)
         from blockValue in getBlock(state)
         select (comparisandValue, sourceValue, blockValue);
      if (_result is (true, var (comparisand, source, block)))
      {
         state.AddStatement(new For(comparisand, source, block));
         return unit;
      }
      else
      {
         return _result.Exception;
      }
   }
}