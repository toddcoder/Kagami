using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class SliceAssignParser : SymbolParser
   {
      public SliceAssignParser(ExpressionBuilder builder) : base(builder)
      {
      }

      public override string Pattern => "^ /'{'";

      public override Responding<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.BeginTransaction();
         state.Colorize(tokens, Color.Structure);

         var _symbol =
            from indexes in getExpression(state, "^ /(/s*) /'}' /(/s*) /'='", builder.Flags, Color.Whitespace, Color.Structure,
               Color.Whitespace, Color.Structure)
            from values in getExpression(state, builder.Flags)
            select new SliceAssignSymbol(indexes, values);
         if (_symbol)
         {
            builder.Add(_symbol);
            state.CommitTransaction();

            return unit;
         }
         else if (_symbol.AnyException)
         {
            state.RollBackTransaction();
            return _symbol.Exception;
         }
         else
         {
            state.RollBackTransaction();
            return nil;
         }
      }
   }
}