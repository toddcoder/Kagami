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

   [GeneratedRegex(@"^(\s+)(if|map|join|sort|foldl|foldr|fold|all|any|none|one|zip|Z|skip|take|while|until|min|max" +
      "|does|X|each|approx|same|xor|union|intersect|diff|symdiff|subsetof|supersetof|accum|overlaps|to|till|downto|" +
      @"downtill|dto|dtill|by|range)(\s+)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      var keyword = tokens[2].Text;
      if ((builder.Flags[ExpressionFlags.OmitRange] || builder.Flags[ExpressionFlags.InLambda]) && keyword != "div" && keyword != "divmod" &&
          keyword != "min" && keyword != "max" && keyword != "to" && keyword != "till" && keyword != "dto" && keyword != "dtill" && keyword != "by" ||
          builder.Flags[ExpressionFlags.OmitWhileUntil] && keyword is "while" or "until")
      {
         return nil;
      }
      else
      {
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
                  builder.Add(new SendBinaryMessageSymbol($"{keyword}(_)", Precedence.ChainedOperator));
                  break;
               case "sort":
                  builder.Add(new SendBinaryMessageSymbol("sort(_<Lambda>)", Precedence.ChainedOperator));
                  break;
               case "foldl":
               case "fold":
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
               case "while" when !builder.Flags[ExpressionFlags.OmitWhileUntil]:
                  builder.Add(new SendBinaryMessageSymbol("takeWhile(_<Lambda>)", Precedence.ChainedOperator));
                  break;
               case "while":
                  return nil;
               case "until" when !builder.Flags[ExpressionFlags.OmitWhileUntil]:
                  builder.Add(new SendBinaryMessageSymbol("takeUntil(_)", Precedence.ChainedOperator));
                  break;
               case "until":
                  return nil;
               case "min":
                  builder.Add(new MinSymbol());
                  break;
               case "max":
                  builder.Add(new MaxSymbol());
                  break;
               case "does":
                  builder.Add(new SendBinaryMessageSymbol("respondsTo(_)", Precedence.Boolean));
                  break;
               case "X":
                  builder.Add(new SendBinaryMessageSymbol("cross(_)", Precedence.Concatenate));
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
               case "union":
                  builder.Add(new SendBinaryMessageSymbol("union(_)", Precedence.AddSubtract));
                  break;
               case "diff":
                  builder.Add(new SendBinaryMessageSymbol("difference(_)", Precedence.AddSubtract));
                  break;
               case "intersect":
                  builder.Add(new SendBinaryMessageSymbol("intersection(_)", Precedence.MultiplyDivide));
                  break;
               case "symdiff":
                  builder.Add(new SendBinaryMessageSymbol("symmetricDifference(_)", Precedence.MultiplyDivide));
                  break;
               case "supersetof":
                  builder.Add(new SendBinaryMessageSymbol("isSupersetOf(_)", Precedence.Boolean));
                  break;
               case "subsetof":
                  builder.Add(new SendBinaryMessageSymbol("isSubsetOf(_)", Precedence.Boolean));
                  break;
               case "accum":
                  builder.Add(new SendBinaryMessageSymbol("accumulate(_)", Precedence.ChainedOperator));
                  break;
               case "overlaps":
                  builder.Add(new SendBinaryMessageSymbol("overlaps(_)", Precedence.Boolean));
                  break;
               case "to":
                  builder.Add(new RangeSymbol(true, false));
                  break;
               case "till":
                  builder.Add(new RangeSymbol(false, false));
                  break;
               case "downto" or "dto":
                  builder.Add(new RangeSymbol(true, true));
                  break;
               case "downtill" or "dtill":
                  builder.Add(new RangeSymbol(false, true));
                  break;
               case "by":
                  builder.Add(new SendBinaryMessageSymbol("by(_<Int>)", Precedence.Range));
                  break;
               case "Z":
                  builder.Add(new SendBinaryMessageSymbol("zip(_)", Precedence.ChainedOperator));
                  break;
               case "range":
                  builder.Add(new OpenRangeSymbol());
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