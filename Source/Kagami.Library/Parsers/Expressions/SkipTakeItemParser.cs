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
         Suffix = nil;
         Prefix = nil;
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
               return _expression.Exception;
            }
            else
            {
               Prefix = nil;
            }
         }
         else
         {
            Prefix = nil;
         }

         if (state.Scan("^ /(|s|) /'~'", Color.Whitespace, Color.Operator))
         {
            var _expression = getExpression(state, ExpressionFlags.OmitComma | ExpressionFlags.OmitConcatenate);
            if (_expression)
            {
               Suffix = _expression.Maybe();
            }
            else if (_expression.AnyException)
            {
               return _expression.Exception;
            }
            else
            {
               Suffix = nil;
            }
         }
         else
         {
            Suffix = nil;
         }

         return unit;
      }

      public int Skip { get; set; }

      public int Take { get; set; }

      public Maybe<Expression> Prefix { get; set; }

      public Maybe<Expression> Suffix { get; set; }
   }
}