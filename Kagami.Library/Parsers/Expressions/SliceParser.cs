using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class SliceParser : SymbolParser
{
   public class SkipTake
   {
      public Maybe<Expression> Skip { get; set; } = nil;

      public Maybe<Expression> Take { get; set; } = nil;

      public bool Terminal { get; set; }

      public override string ToString()
      {
         return $"{Skip.Map(e => e.ToString()) | ""};{Take.Map(e => e.ToString()) | ""}";
      }
   }

   public SliceParser(ExpressionBuilder builder) : base(builder)
   {
   }

//   public override string Pattern => "^ /'{'";

   [GeneratedRegex(@"^({)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.OpenParenthesis);

      List<SkipTake> skipTakes = [];

      while (state.More)
      {
         var _skipTake = getSkipTake(state, builder.Flags | ExpressionFlags.OmitComma);
         if (_skipTake is (true, var skipTake))
         {
            skipTakes.Add(skipTake);
            if (skipTake.Terminal)
            {
               break;
            }
         }
         else if (_skipTake.Exception is (true, var exception))
         {
            return exception;
         }
      }

      builder.Add(new SliceSymbol([.. skipTakes]));
      return unit;
   }

   protected Optional<SkipTake> getSkipTake(ParseState state, ExpressionFlags flags)
   {
      var skipTake = new SkipTake();

      var _noSkipMatch = state.Scan(@"^(\s*)(,)", Color.Whitespace, Color.Structure);
      if (_noSkipMatch)
      {
      }
      else if (_noSkipMatch.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         var _skipExpression = getExpression(state, flags);
         if (_skipExpression is (true, var skipExpression))
         {
            skipTake.Skip = skipExpression;
         }
         else if (_skipExpression.Exception is (true, var exception2))
         {
            return exception2;
         }

         var _semiOrEnd = state.Scan(@"^(\s*)([;,}])", Color.Whitespace, Color.CloseParenthesis);
         if (_semiOrEnd is (true, var semiOrEnd))
         {
            switch (semiOrEnd)
            {
               case "}":
                  skipTake.Terminal = true;
                  return skipTake;
               case ";":
                  return skipTake;
            }
         }
         else if (_semiOrEnd.Exception is (true, var exception3))
         {
            return exception3;
         }
      }

      var _takeExpression = getExpression(state, flags);
      if (_takeExpression is (true, var takeExpression))
      {
         skipTake.Take = takeExpression;
      }
      else if (_takeExpression.Exception is (true, var exception))
      {
         return exception;
      }

      var _end = state.Scan(@"^(\s*)([};])", Color.Whitespace, Color.CloseParenthesis);
      if (_end is (true, var end))
      {
         switch (end)
         {
            case "}":
               skipTake.Terminal = true;
               return skipTake;
         }
      }
      else if (_end.Exception is (true, var exception4))
      {
         return exception4;
      }

      return skipTake;
   }
}