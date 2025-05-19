using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class SeqParser : SymbolParser
{
   public SeqParser(ExpressionBuilder builder) : base(builder)
   {
   }

   public override string Pattern => $"^ /(|s|) /'seq' /({REGEX_EOL})";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);
      state.CreateYieldFlag();

      var _block = getBlock(state);
      if (_block is (true, var block))
      {
         var yielding = state.RemoveYieldFlag();
         if (yielding)
         {
            builder.Add(new SeqSymbol(block));
            return unit;
         }
         else
         {
            return fail("Yield required");
         }
      }
      else
      {
         return _block.Exception;
      }
   }
}