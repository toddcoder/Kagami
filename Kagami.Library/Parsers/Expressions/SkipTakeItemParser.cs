using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public class SkipTakeItemParser : Parser
{
   public SkipTakeItemParser() : base(false)
   {
   }

   public override string Pattern => "^ /(|s|) /((['+-'] [/d '_']+) | '0')? /':' /((['+-'] [/d '_']+) | '0')?";

   public override Optional<Unit> Parse(ParseState state, Token[] tokens)
   {
      var skip = tokens[2].Text.Replace("_", "").DefaultTo("0");
      var take = tokens[4].Text.Replace("_", "").DefaultTo("0");
      state.Colorize(tokens, Color.Whitespace, Color.Number, Color.Operator, Color.Number);

      Skip = skip.Value().Int32();
      Take = take.Value().Int32();

      if (state.Scan("^ /(|s|) /'='", Color.Whitespace, Color.Operator))
      {
         var _expression = getExpression(state, ExpressionFlags.OmitComma);
         if (_expression is (true, var expression))
         {
            Prefix = expression;
         }
         else if (_expression.Exception is (true, var exception))
         {
            return exception;
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
         if (_expression is (true, var expression))
         {
            Suffix = expression;
         }
         else if (_expression.Exception is (true, var exception))
         {
            return exception;
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

   public Maybe<Expression> Prefix { get; set; } = nil;

   public Maybe<Expression> Suffix { get; set; } = nil;
}