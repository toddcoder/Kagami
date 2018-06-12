using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class IfSomeNilSymbolParser : SymbolParser
   {
      public IfSomeNilSymbolParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'||'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.BeginTransaction();

         state.Colorize(tokens, Color.Whitespace, Color.Structure);

         if (getExpression(state, builder.Flags).If(out var expression, out var isNotMatched, out var exception))
         {
            state.CommitTransaction();
            builder.Add(new IfSomeNilSymbol(expression));

            return Unit.Matched();
         }
         else if (isNotMatched)
         {
            state.RollBackTransaction();
            return notMatched<Unit>();
         }
         else
            return failedMatch<Unit>(exception);
      }
   }
}