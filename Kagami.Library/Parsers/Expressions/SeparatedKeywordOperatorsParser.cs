using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using Standard.Types.Strings;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SeparatedKeywordOperatorsParser : SymbolParser
   {
      public SeparatedKeywordOperatorsParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /('skip' | 'take' | 'if') /(|s+|) /('while' | 'until' | 'not') /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var word1 = tokens[1].Text;
         var word2 = tokens[3].Text;
         state.Colorize(tokens, Color.Keyword, Color.Whitespace, Color.Operator);

         switch (word1)
         {
            case "skip":
               switch (word2)
               {
                  case "while":
                  case "until":
                     builder.Add(new SendBinaryMessageSymbol($"{word1}{word2.ToTitleCase()}", Precedence.ChainedOperator));
                     break;
                  default:
                     return notMatched<Unit>();
               }

               break;
            case "take":
               switch (word2)
               {
                  case "while":
                  case "until":
                     builder.Add(new SendBinaryMessageSymbol($"{word1}{word2.ToTitleCase()}", Precedence.ChainedOperator));
                     break;
                  default:
                     return notMatched<Unit>();
               }

               break;
            case "if":
               if (word2 == "not")
                  builder.Add(new SendBinaryMessageSymbol("ifNot", Precedence.ChainedOperator));
               else
                  return notMatched<Unit>();

               break;
         }

         return Unit.Matched();
      }
   }
}