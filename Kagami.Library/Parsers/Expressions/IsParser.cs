using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public class IsParser : SymbolParser
   {
      static IMatched<IMaybe<Expression>> optionalExpression(ParseState state, ExpressionFlags flags)
      {
         var result =
            from colon in state.Scan("^ /(/s*) /'else' /b", Color.Whitespace, Color.Keyword)
            from expression in getExpression(state, flags)
            select expression;
         if (result.ValueOrCast<IMaybe<Expression>>(out var expressionValue, out var original))
         {
            return expressionValue.Some().Matched();
         }
         else if (original.IsNotMatched)
         {
            return none<Expression>().Matched();
         }
         else
         {
            return original.ExceptionAs<IMaybe<Expression>>();
         }
      }

      public IsParser(ExpressionBuilder builder) : base(builder) { }

      public override string Pattern => "^ /(|s+|) /'on' /b";

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         state.Colorize(tokens, Color.Whitespace, Color.Keyword);
         var flags = builder.Flags;
         var result =
            from comparisand in getExpression(state, ExpressionFlags.Comparisand)
            from scanned in state.Scan("^ /(/s*) /'then' /b", Color.Whitespace, Color.Keyword)
            from expression in getExpression(state, flags)
            from elseExpression in optionalExpression(state, flags)
            select (comparisand, expression, elseExpression);
         if (result.ValueOrCast<Unit>(out var tuple, out var asUnit))
         {
            builder.Add(new IsSymbol(tuple.comparisand, tuple.expression, tuple.elseExpression));
            return Unit.Matched();
         }
         else
         {
            return asUnit.Unmatched<Unit>();
         }
      }
   }
}