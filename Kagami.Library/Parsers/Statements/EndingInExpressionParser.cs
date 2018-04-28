using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public abstract class EndingInExpressionParser : StatementParser
   {
      public abstract IMatched<Unit> Prefix(ParseState state, Token[] tokens);

      public abstract IMatched<Unit> Suffix(ParseState state, Expression expression);

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         if (Prefix(state, tokens).If(out _, out var isNotMatched, out var exception))
            if (getExpression(state, ExpressionFlags.Standard).If(out var expression, out isNotMatched, out exception))
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