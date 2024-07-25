using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class PartialLambdaParser : SymbolParser
   {
      public PartialLambdaParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'('";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.BeginTransaction();
         state.Colorize(tokens, Color.Whitespace, Color.OpenParenthesis);
         if (getPartialLambda(state).ValueOrCast<Unit>(out var lambda, out var asUnit))
         {
            builder.Add(lambda);
            state.CommitTransaction();

            return Unit.Matched();
         }
         else
         {
            state.RollBackTransaction();
            return asUnit;
         }
      }
   }
}