using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class LazyParser : SymbolParser
{
   public LazyParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s*)(lazy)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.Whitespace, Color.Keyword);

      if (state.BlockFollows())
      {
         state.CreateYieldFlag();
         state.CreateReturnType();

         var _block = getBlock(state);
         if (_block is (true, var block))
         {
            state.GetReturnType();
            state.RemoveYieldFlag();

            builder.Add(new LazyBlockSymbol(block));
            return unit;
         }
         else
         {
            return _block.Exception;
         }
      }
      else
      {
         var _expression = getExpression(state, ExpressionFlags.Standard);
         if (_expression is (true, var expression))
         {
            builder.Add(new LazySymbol(expression));
            return unit;
         }
         else
         {
            return _expression.Exception;
         }
      }
   }
}