using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class FlexRangeParser : SymbolParser
   {
      public FlexRangeParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'from' /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword);

         var prefix =
            from fromValue in getExpression(state, builder.Flags | ExpressionFlags.OmitRange)
            from byKeyword in state.Scan("^ /(|s+|) /'by' /b", Color.Whitespace, Color.Keyword)
            from lambda in getAnyLambda(state, builder.Flags)
            select (fromValue, lambda);
         if (prefix.If(out var tuple, out var original))
         {
            var (fromValue, lambda) = tuple;
            var suffix =
               from toKeyword in state.Scan("^ /(|s+|) /('to' | 'til') /b", Color.Whitespace, Color.Keyword)
               from toExpression in getExpression(state, builder.Flags)
               select (toKeyword, toExpression);
            var to = none<Expression>();
            var inclusive = false;
            if (suffix.If(out var toTuple, out var isNotMatched, out var exception))
            {
               var (toKeyword, toExpression) = toTuple;
               to = toExpression.Some();
               inclusive = toKeyword.Contains("to");
            }
            else if (!isNotMatched)
               return failedMatch<Unit>(exception);

            builder.Add(new FlexRangeSymbol(fromValue, lambda, to, inclusive));
            return Unit.Matched();
         }
         else
            return original.Unmatched<Unit>();
      }
   }
}