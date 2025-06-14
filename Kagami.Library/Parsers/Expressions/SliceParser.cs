using System.Text.RegularExpressions;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions;

public partial class SliceParser : SymbolParser
{
   public SliceParser(ExpressionBuilder builder) : base(builder)
   {
   }

   [GeneratedRegex("^({)")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
   {
      state.Colorize(tokens, Color.OpenParenthesis);

      List<SkipTake> skipTakes = [];

      while (state.More)
      {
         var _skipTake = getSkipTake(state, builder.Flags | ExpressionFlags.OmitColon | ExpressionFlags.OmitBind);
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
}