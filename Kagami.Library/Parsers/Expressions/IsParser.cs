using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class IsParser : SymbolParser
   {
      public IsParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s+|) /'is' /(|s+| 'not') /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
	      var not = tokens[3].Text.Length > 0;
	      state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Keyword);
         return getExpression(state, ExpressionFlags.Comparisand).Map(e =>
         {
            builder.Add(new IsSymbol(e, not));
            return Unit.Value;
         });
      }
   }
}