using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ListParser : SymbolParser
   {
      public ListParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(/s*) /'[' /(/s*)";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Collection, Color.Whitespace);

         if (getExpression(state, "^ /(/s*) /']'", builder.Flags & ~ExpressionFlags.OmitComma, Color.Whitespace, Color.Collection)
            .If(out var expression, out var original))
         {
            builder.Add(new ListSymbol(expression));
            return Unit.Matched();
         }
         else
            return original.Unmatched<Unit>();
      }
   }
}