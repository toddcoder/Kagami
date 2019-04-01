using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class TwoKeywordOperatorsParser : SymbolParser
   {
      public TwoKeywordOperatorsParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /('skip' | 'take') /(|s+|) /('while' | 'until') /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.BeginTransaction();

         var word1 = tokens[2].Text;
         var word2 = tokens[4].Text;
         var message = "";
         state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Keyword);

         switch (word1)
         {
            case "skip":
            case "take":
               switch (word2)
               {
                  case "while":
                  case "until":
                     message = word1.Selector($"{word2}:");
                     break;
               }
               break;
            default:
               state.RollBackTransaction();
               return notMatched<Unit>();
         }

         builder.Add(new SendBinaryMessageSymbol(message, Precedence.ChainedOperator));
         return Unit.Matched();
      }
   }
}