using System.Collections.Generic;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SliceParser : SymbolParser
   {
      public class SkipTake
      {
         public SkipTake()
         {
            Take = none<Expression>();
            Skip = none<Expression>();
         }

         public IMaybe<Expression> Skip { get; set; }

         public IMaybe<Expression> Take { get; set; }

         public bool Terminal { get; set; }

         public override string ToString()
         {
            return $"{Skip.Map(e => e.ToString()).DefaultTo(() => "")};{Take.Map(e => e.ToString()).DefaultTo(() => "")}";
         }
      }

      public SliceParser(ExpressionBuilder builder) : base(builder)
      {
      }

      public override string Pattern => "^ /'{'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.OpenParenthesis);

         var skipTakes = new List<SkipTake>();

         while (state.More)
         {
            var skipTakeMatch = getSkipTake(state, builder.Flags | ExpressionFlags.OmitComma);
            if (skipTakeMatch.If(out var skipTake, out var _exception))
            {
               skipTakes.Add(skipTake);
               if (skipTake.Terminal)
               {
                  break;
               }
            }
            else if (_exception.If(out var exception))
            {
               return failedMatch<Unit>(exception);
            }
         }

         builder.Add(new SliceSymbol(skipTakes.ToArray()));

         return Unit.Matched();
      }

      protected IMatched<SkipTake> getSkipTake(ParseState state, ExpressionFlags flags)
      {
         var skipTake = new SkipTake();

         var noSkipMatch = state.Scan("^ /(|s|) /','", Color.Whitespace, Color.Structure);
         if (noSkipMatch.If(out _, out var _exception))
         {
         }
         else if (_exception.If(out var exception))
         {
            return failedMatch<SkipTake>(exception);
         }
         else
         {
            var skipMatch = getExpression(state, flags);
            if (skipMatch.If(out var skipExpression, out _exception))
            {
               skipTake.Skip = skipExpression.Some();
            }
            else if (_exception.If(out exception))
            {
               return failedMatch<SkipTake>(exception);
            }

            var semiOrEndMatch = state.Scan("^ /(|s|) /[';,}']", Color.Whitespace, Color.CloseParenthesis);
            if (semiOrEndMatch.If(out var semiOrEnd, out _exception))
            {
               switch (semiOrEnd)
               {
                  case "}":
                     skipTake.Terminal = true;
                     return skipTake.Matched();
                  case ";":
                     return skipTake.Matched();
               }
            }
            else if (_exception.If(out exception))
            {
               return failedMatch<SkipTake>(exception);
            }
         }

         var takeMatch = getExpression(state, flags);
         if (takeMatch.If(out var takeExpression, out _exception))
         {
            skipTake.Take = takeExpression.Some();
         }
         else if (_exception.If(out var exception))
         {
            return failedMatch<SkipTake>(exception);
         }

         var endMatch = state.Scan("^ /(|s|) /['};']", Color.Whitespace, Color.CloseParenthesis);
         if (endMatch.If(out var end, out _exception))
         {
            switch (end)
            {
               case "}":
                  skipTake.Terminal = true;
                  return skipTake.Matched();
            }
         }
         else if (_exception.If(out var exception))
         {
            return failedMatch<SkipTake>(exception);
         }

         return skipTake.Matched();
      }
   }
}