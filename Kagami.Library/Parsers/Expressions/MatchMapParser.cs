using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class MatchMapParser : SymbolParser
   {
      public MatchMapParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s|) /'||'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.BeginTransaction();

         state.Colorize(tokens, Color.Whitespace, Color.Structure);

         var result =
            from comparisand in getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitConjunction)
            from arrow in state.Scan("^ /(|s+|) /'||'", Color.Whitespace, Color.Structure)
            from expression in getExpression(state, builder.Flags)
            select (comparisand, expression);

         if (result.If(out var tuple, out var isNotMatched, out var exception))
         {
            state.CommitTransaction();
            var (comparisand, expression) = tuple;
            builder.Add(new MatchMapSymbol(comparisand, expression));

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