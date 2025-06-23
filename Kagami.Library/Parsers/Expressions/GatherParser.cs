using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class GatherParser : SymbolParser
{
   public GatherParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"(\s*)(gather)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      state.CreateYieldFlag();
      state.CreateReturnType();

      var _block = getBlock(state);
      if (_block is (true, var block))
      {
         state.RemoveYieldFlag();
         state.RemoveReturnType();
         builder.Add(new GatherSymbol(block));
         return unit;
      }
      else if (_block.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }
}