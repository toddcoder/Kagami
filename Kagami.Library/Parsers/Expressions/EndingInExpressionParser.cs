using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Expressions
{
   public abstract class EndingInExpressionParser : SymbolParser
   {
      ExpressionFlags flags;

      public EndingInExpressionParser(ExpressionBuilder builder, ExpressionFlags flags = ExpressionFlags.Standard) : base(builder)
      {
         this.flags = flags;
      }

      public abstract IMatched<Unit> Prefix(ParseState state, Token[] tokens);

      public abstract IMatched<Unit> Suffix(ParseState state, Expression expression);

      public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
      {
         if (Prefix(state, tokens).If(out _, out var isNotMatched, out var exception))
            if (getExpression(state, builder.Flags | flags).If(out var expression, out isNotMatched, out exception))
               return Suffix(state, expression);
            else if (isNotMatched)
               return failedMatch<Unit>(expectedExpression());
            else
               return failedMatch<Unit>(exception);
         else if (isNotMatched)
            return failedMatch<Unit>(expectedExpression());
         else
            return failedMatch<Unit>(exception);
      }
   }
}