using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class KeywordOperatorsParser : SymbolParser
{
   public KeywordOperatorsParser(ExpressionBuilder builder) : base(builder) { }

   public override string Pattern => "^ /(|s|) /('to' | 'til' | 'by' | 'if' | 'map' | 'join' | 'sort' |" +
      "'foldl' | 'foldr' | 'all' | 'any' | 'none' | 'one' | 'zip' | 'downto' | 'skip' | 'take' | 'band' | 'bor' |" +
      " 'bxor' | 'bsl' | 'bsr' | 'while' | 'until' | 'min' | 'max' | 'does' | 'x') /(/s+)";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      if (builder.Flags[ExpressionFlags.OmitRange])
      {
         return nil;
      }
      else
      {
         var keyword = tokens[2].Text;
         if (keyword != "if" || !builder.Flags[ExpressionFlags.OmitIf])
         {
            state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);

            switch (keyword)
            {
               case "to":
               case "by":
                  builder.Add(new RangeSymbol(true));
                  break;
               case "til":
                  builder.Add(new RangeSymbol(false));
                  break;
               case "if":
               case "map":
               case "join":
               case "all":
               case "any":
               case "none":
               case "one":
               case "zip":
                  builder.Add(new SendBinaryMessageSymbol(keyword, Precedence.ChainedOperator));
                  break;
               case "sort":
                  builder.Add(new SendBinaryMessageSymbol("sort".Selector("<Lambda>"), Precedence.ChainedOperator));
                  break;
               case "foldl":
                  builder.Add(new SendBinaryMessageSymbol("foldl", Precedence.ChainedOperator));
                  break;
               case "foldr":
                  builder.Add(new SendBinaryMessageSymbol("foldr", Precedence.ChainedOperator));
                  break;
               case "downto":
                  builder.Add(new RangeSymbol(true));
                  break;
               case "skip":
                  builder.Add(new SendBinaryMessageSymbol("skip", Precedence.ChainedOperator));
                  break;
               case "take":
                  builder.Add(new SendBinaryMessageSymbol("take", Precedence.ChainedOperator));
                  break;
               case "band":
                  builder.Add(new BAndSymbol());
                  break;
               case "bor":
                  builder.Add(new BOrSymbol());
                  break;
               case "bxor":
                  builder.Add(new BXorSymbol());
                  break;
               case "bsl":
                  builder.Add(new BShiftLeft());
                  break;
               case "bsr":
                  builder.Add(new BShiftRight());
                  break;
               case "while":
               case "until":
                  builder.Add(new SendBinaryMessageSymbol($"take{keyword.ToTitleCase()}(_<Lambda>)", Precedence.ChainedOperator));
                  break;
               case "min":
                  builder.Add(new MinSymbol());
                  break;
               case "max":
                  builder.Add(new MaxSymbol());
                  break;
               case "does":
                  builder.Add(new SendBinaryMessageSymbol("respondsTo", Precedence.Boolean));
                  break;
               case "x":
                  builder.Add(new SendBinaryMessageSymbol("cross(_)", Precedence.Concatenate));
                  break;
               default:
                  return fail($"Keyword internal error for {keyword}");
            }

            return unit;
         }
         else
         {
            return nil;
         }
      }
   }
}