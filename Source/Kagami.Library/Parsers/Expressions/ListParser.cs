using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class ListParser : SymbolParser
   {
      public ListParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => $"^ /(/s*) /'{REGEX_LIST_LEFT}' /(/s*)";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Collection, Color.Whitespace);

         if (getExpression(state, $"^ /(/s*) /'{REGEX_LIST_RIGHT}'", builder.Flags & ~ExpressionFlags.OmitComma, Color.Whitespace,
            Color.Collection).ValueOrCast<Unit>(out var expression, out var asUnit))
         {
            builder.Add(new ListSymbol(expression));
            return Unit.Matched();
         }
         else
         {
            return asUnit.Unmatched<Unit>();
         }
      }
   }
}