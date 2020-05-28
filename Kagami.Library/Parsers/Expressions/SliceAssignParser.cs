using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SliceAssignParser : SymbolParser
   {
      public SliceAssignParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /'{'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.BeginTransaction();
         state.Colorize(tokens, Color.Structure);

         var result =
            from indexes in getExpression(state, "^ /(/s*) /'}' /(/s*) /'='", builder.Flags, Color.Whitespace, Color.Structure,
               Color.Whitespace, Color.Structure)
            from values in getExpression(state, builder.Flags)
            select new SliceAssignSymbol(indexes, values);
         if (result.ValueOrCast<Unit>(out var symbol, out var asUnit))
         {
            builder.Add(symbol);
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