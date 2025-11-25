using Core.Monads;
using Core.Strings;
using Kagami.Library.Nodes.Symbols;
using System.Text.RegularExpressions;
using static Core.Monads.MonadFunctions;
using Regex = System.Text.RegularExpressions.Regex;

namespace Kagami.Library.Parsers.Expressions;

public partial class InternalKeywordOperatorsParser : SymbolParser
{
   public InternalKeywordOperatorsParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(
      @"^\b(if|map|join|sort|foldl|foldr|all|any|none|one|zip|skip|take|xor|bsl|bsr|while|until|min|max|does|x|each)\b")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      if (builder.Flags[ExpressionFlags.OmitRange])
      {
         return nil;
      }
      else
      {
         var keyword = tokens[1].Text;
         if (keyword != "if" || !builder.Flags[ExpressionFlags.OmitIf])
         {
            state.Colorize(tokens, Color.Operator);

            switch (keyword)
            {
               case "if":
               case "map":
               case "join":
               case "all":
               case "any":
               case "none":
               case "one":
               case "zip":
               case "each":
                  builder.Add(new SendBinaryMessageSymbol($"{keyword}(_)", Precedence.ChainedOperator));
                  break;
               case "sort":
                  builder.Add(new SendBinaryMessageSymbol("sort".Selector("<Lambda>"), Precedence.ChainedOperator));
                  break;
               case "foldl":
                  builder.Add(new SendBinaryMessageSymbol("foldl(_)", Precedence.ChainedOperator));
                  break;
               case "foldr":
                  builder.Add(new SendBinaryMessageSymbol("foldr(_)", Precedence.ChainedOperator));
                  break;
               case "skip":
                  builder.Add(new SendBinaryMessageSymbol("skip(_)", Precedence.ChainedOperator));
                  break;
               case "take":
                  builder.Add(new SendBinaryMessageSymbol("take(_)", Precedence.ChainedOperator));
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
                  builder.Add(new SendBinaryMessageSymbol("respondsTo(_)", Precedence.Boolean));
                  break;
               case "x":
                  builder.Add(new SendBinaryMessageSymbol("cross(_)", Precedence.Concatenate));
                  break;
               /*case "r":
                  builder.Add(new RationalSymbol());
                  break;*/
               case "xor":
                  state.PrefixCode = nil;
                  builder.Add(new XOrSymbol());
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