using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class IsParser : SymbolParser
   {
      public IsParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s+|) /'is' /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword);
         return getExpression(state, ExpressionFlags.Comparisand).Map(e =>
         {
            builder.Add(new IsSymbol(e));
            return Unit.Value;
         });
      }
   }
}