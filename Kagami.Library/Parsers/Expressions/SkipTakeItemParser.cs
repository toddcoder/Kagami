using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ConversionFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SkipTakeItemParser : Parser
   {
      public SkipTakeItemParser() : base(false)
      {
      }

      public override string Pattern => "^ /(|s|) /((['+-'] [/d '_']+) | '0')? /':' /((['+-'] [/d '_']+) | '0')?";

      public override Responding<Unit> Parse(ParseState state, Token[] tokens)
      {
         var skip = tokens[2].Text.Replace("_", "").DefaultTo("0");
         var take = tokens[4].Text.Replace("_", "").DefaultTo("0");
         state.Colorize(tokens, Color.Whitespace, Color.Number, Color.Operator, Color.Number);

         Skip = Value.Int32(skip);
         Take = Value.Int32(take);

         if (state.Scan("^ /(|s|) /'='", Color.Whitespace, Color.Operator))
         {
            var _expression = getExpression(state, ExpressionFlags.OmitComma);
            if (_expression)
            {
               Prefix = _expression.Value;
            }
            else if (_expression.AnyException)
            {
               return 
            }
            else
            {
               Prefix = none<Expression>();
            }
         }
         else
         {
            Prefix = none<Expression>();
         }

         if (state.Scan("^ /(|s|) /'~'", Color.Whitespace, Color.Operator).IsMatched)
         {
            if (getExpression(state, ExpressionFlags.OmitComma | ExpressionFlags.OmitConcatenate)
                .If(out var expression, out var anyException))
            {
               Suffix = expression.Some();
            }
            else if (anyException.If(out var exception))
            {
               return failedMatch<Unit>(exception);
            }
            else
            {
               Suffix = none<Expression>();
            }
         }
         else
         {
            Suffix = none<Expression>();
         }

         return Unit.Matched();
      }

      public int Skip { get; set; }

      public int Take { get; set; }

      public Maybe<Expression> Prefix { get; set; } = none<Expression>();

      public Maybe<Expression> Suffix { get; set; } = none<Expression>();
   }
}