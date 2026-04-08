using System.Text.RegularExpressions;
using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class BlockValueParser : SymbolParser
{
   public BlockValueParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(\.{)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Block);

      var _block = getPartialBlock(state, nil);
      if (_block is (true, var block))
      {
         var blockValueSymbol = new BlockValueSymbol(block);
         builder.Add(blockValueSymbol);

         return unit;
      }
      else
      {
         return _block.Exception;
      }
   }
}