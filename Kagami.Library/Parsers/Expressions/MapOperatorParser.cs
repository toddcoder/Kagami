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
            switch (expression.Symbols[0])
            {
               case LambdaSymbol lambdaSymbol:
                  builder.Add(new MapOperatorSymbol(lambdaSymbol));
                  return Unit.Matched();
               case SubexpressionSymbol subexpression when subexpression.Expression.Symbols[0] is LambdaSymbol innerLambdaSymbol:
                  builder.Add(new MapOperatorSymbol(innerLambdaSymbol));
                  return Unit.Matched();
               default:
                  return "Lambda required".FailedMatch<Unit>();
            }
         else
            return original.UnmatchedOnly<Unit>();
      }
   }
}