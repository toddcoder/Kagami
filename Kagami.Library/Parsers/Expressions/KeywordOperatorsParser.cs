using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class KeywordOperatorsParser : SymbolParser
{
   public KeywordOperatorsParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex(@"^(\s+)(if|map|join|sort|foldl|foldr|all|any|none|one|zip|skip|take|while|until|min|max" +
      @"|does|x|div|each|divmod|with|approx|same|xor)(\s+)")]
   public override partial Regex Regex();

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
            state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Whitespace);

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
               case "with":
                  builder.Add(new SendBinaryMessageSymbol($"{keyword}(_)", Precedence.ChainedOperator));
                  break;
               case "sort":
                  builder.Add(new SendBinaryMessageSymbol("sort(_<Lambda>)", Precedence.ChainedOperator));
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
                  builder.Add(new SendBinaryMessageSymbol("takeWhile(_<Lambda>)", Precedence.ChainedOperator));
                  break;
               case "until":
                  builder.Add(new SendBinaryMessageSymbol("takeUntil(_)", Precedence.ChainedOperator));
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
               case "div":
                  builder.Add(new IntDivideSymbol());
                  break;
               case "divmod":
                  builder.Add(new DivModSymbol());
                  break;
               case "approx":
                  builder.Add(new ApproximateSymbol());
                  break;
               case "same":
                  builder.Add(new SameSymbol(false));
                  break;
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