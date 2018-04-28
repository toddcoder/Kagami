using Standard.Types.Maybe;
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
         state.Colorize(tokens, Color.Whitespace, Color.Structure);
         if (getPartialLambda(state).If(out var lambda, out var original))
         {
            builder.Add(lambda);
            state.CommitTransaction();

            return Unit.Matched();
         }
         else
         {
            state.RollBackTransaction();
            return original.Unmatched<Unit>();
         }
      }
   }
}