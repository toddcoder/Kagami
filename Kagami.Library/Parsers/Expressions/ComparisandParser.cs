using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ComparisandParser : SymbolParser
   {
      public ComparisandParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'|'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Operator);
         if (getTerm(state, ExpressionFlags.Comparisand).ValueOrCast<Unit>(out var expression, out var asUnit))
         {
            builder.Add(expression);
            return Unit.Matched();
         }
         else
         {
            return asUnit;
         }
      }
   }
}