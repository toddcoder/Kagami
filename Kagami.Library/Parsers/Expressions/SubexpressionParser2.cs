using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SubexpressionParser2 : SymbolParser
   {
      public SubexpressionParser2(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'('";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.BeginTransaction();
         state.Colorize(tokens, Color.Whitespace, Color.Structure);

         if (getExpression(state, "^ /')'", builder.Flags & ~ExpressionFlags.OmitComma, Color.Structure)
            .If(out var expression, out var isNotMatched, out _))
         {
            builder.Add(new SubexpressionSymbol(expression));
            state.CommitTransaction();

            return Unit.Matched();
         }
         else if (isNotMatched)
            return notMatched<Unit>();
         else
         {
            state.RollBackTransaction();
            state.BeginTransaction();
            if (getPartialLambda(state).If(out var lambdaSymbol, out var original))
            {
               state.CommitTransaction();
               builder.Add(lambdaSymbol);

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
}