using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using Standard.Types.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SkipTakeItemParser : Parser
   {
      public SkipTakeItemParser() : base(false) { }

      public override string Pattern => "^ /(|s|) /((['+-'] [/d '_']+) | '0')? /':' /((['+-'] [/d '_']+) | '0')?";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens)
      {
         var skip = tokens[2].Text.Replace("_", "").DefaultTo("0");
         var take = tokens[4].Text.Replace("_", "").DefaultTo("0");
         state.Colorize(tokens, Color.Whitespace, Color.Number, Color.Operator, Color.Number);

         Skip = skip.ToInt();
         Take = take.ToInt();

         if (state.Scan("^ /(|s|) /'='", Color.Whitespace, Color.Operator).IsMatched)
         {
            if (getExpression(state, ExpressionFlags.OmitComma).If(out var expression, out var isNotMatched, out var exception))
               Prefix = expression.Some();
            else if (isNotMatched)
               Prefix = none<Expression>();
            else
               return failedMatch<Unit>(exception);
         }
         else
            Prefix = none<Expression>();

         if (state.Scan("^ /(|s|) /'~'", Color.Whitespace, Color.Operator).IsMatched)
         {
            if (getExpression(state, ExpressionFlags.OmitComma | ExpressionFlags.OmitConcatenate)
               .If(out var expression, out var isNotMatched, out var exception))
               Suffix = expression.Some();
            else if (isNotMatched)
               Suffix = none<Expression>();
            else
               return failedMatch<Unit>(exception);
         }
         else
            Suffix = none<Expression>();

         return Unit.Matched();
      }

      public int Skip { get; set; }

      public int Take { get; set; }

      public IMaybe<Expression> Prefix { get; set; } = none<Expression>();

      public IMaybe<Expression> Suffix { get; set; } = none<Expression>();
   }
}