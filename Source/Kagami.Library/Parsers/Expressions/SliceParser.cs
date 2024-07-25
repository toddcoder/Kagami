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
            Take = nil;
            Skip = nil;
         }

         public Maybe<Expression> Skip { get; set; }

         public Maybe<Expression> Take { get; set; }

         public bool Terminal { get; set; }

         public override string ToString()
         {
            return $"{Skip.Map(e => e.ToString()) | ""};{Take.Map(e => e.ToString()) | ""}";
         }
      }

      public SliceParser(ExpressionBuilder builder) : base(builder)
      {
      }

      public override string Pattern => "^ /'{'";

      public override Responding<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.OpenParenthesis);

         var skipTakes = new List<SkipTake>();

         while (state.More)
         {
            var _skipTake = getSkipTake(state, builder.Flags | ExpressionFlags.OmitComma);
            if (_skipTake)
            {
               skipTakes.Add(_skipTake);
               if (_skipTake.Value.Terminal)
               {
                  break;
               }
            }
            else if (_skipTake.AnyException)
            {
               return _skipTake.Exception;
            }
         }

         builder.Add(new SliceSymbol(skipTakes.ToArray()));

         return unit;
      }

      protected Responding<SkipTake> getSkipTake(ParseState state, ExpressionFlags flags)
      {
         var skipTake = new SkipTake();

         var _noSkipMatch = state.Scan("^ /(|s|) /','", Color.Whitespace, Color.Structure);
         if (_noSkipMatch)
         {
         }
         else if (_noSkipMatch.AnyException)
         {
            return _noSkipMatch.Exception;
         }
         else
         {
            var _skipExpression = getExpression(state, flags);
            if (_skipExpression)
            {
               skipTake.Skip = _skipExpression.Maybe();
            }
            else if (_skipExpression.AnyException)
            {
               return _skipExpression.Exception;
            }

            var _semiOrEnd = state.Scan("^ /(|s|) /[';,}']", Color.Whitespace, Color.CloseParenthesis);
            if (_semiOrEnd)
            {
               switch (_semiOrEnd.Value)
               {
                  case "}":
                     skipTake.Terminal = true;
                     return skipTake;
                  case ";":
                     return skipTake;
               }
            }
            else if (_semiOrEnd.AnyException)
            {
               return _semiOrEnd.Exception;
            }
         }

         var _takeExpression = getExpression(state, flags);
         if (_takeExpression)
         {
            skipTake.Take = _takeExpression.Maybe();
         }
         else if (_takeExpression.AnyException)
         {
            return _takeExpression.Exception;
         }

         var _end = state.Scan("^ /(|s|) /['};']", Color.Whitespace, Color.CloseParenthesis);
         if (_end)
         {
            switch (_end.Value)
            {
               case "}":
                  skipTake.Terminal = true;
                  return skipTake;
            }
         }
         else if (_end.AnyException)
         {
            return _end.Exception;
         }

         return skipTake;
      }
   }
}