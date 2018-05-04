using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class MapOperatorParser : SymbolParser
   {
      public MapOperatorParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /'{'";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Operator);

         if (getExpression(state, "^ /(/s*) /'}'", builder.Flags, Color.Whitespace, Color.Operator)
            .If(out var expression, out var original))
         {
            if (expression.Symbols[0] is LambdaSymbol lambdaSymbol)
            {
               builder.Add(new MapOperatorSymbol(lambdaSymbol));
               return Unit.Matched();
            }
            else
               return "Lambd required".FailedMatch<Unit>();
         }
         else
            return original.UnmatchedOnly<Unit>();
      }
   }
}