using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class FieldParser : SymbolParser
   {
      public FieldParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => $"^ /(|s|) /({REGEX_FIELD}) /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         var source = tokens[2].Text;
         state.Colorize(tokens, Color.Whitespace, Color.Identifier);

         if (state.DefExpression(source).If(out var defExpression))
         {
	         builder.Add(defExpression);
         }
         else
         {
	         builder.Add(builder.Flags[ExpressionFlags.Comparisand] ? (Symbol)new PlaceholderSymbol($"-{source}") : new FieldSymbol(source));
         }

         return Unit.Matched();
      }
   }
}