using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ArrayParser : SymbolParser
   {
      public ArrayParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(/s*) /'[' /(/s*)";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Collection, Color.Whitespace);

         if (getExpression(state, "^ /(/s*) /']'", builder.Flags & ~ExpressionFlags.OmitComma, Color.Whitespace, Color.Collection)
            .ValueOrOriginal(out var expression, out var original))
         {
            builder.Add(new ArraySymbol(expression));
            return Unit.Matched();
         }
         else
         {
            return original.Unmatched<Unit>();
         }
      }
   }
}