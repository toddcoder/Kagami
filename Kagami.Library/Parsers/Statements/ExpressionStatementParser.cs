using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
   public class ExpressionStatementParser : StatementParser
   {
      bool returnExpression;

      public ExpressionStatementParser(bool returnExpression) => this.returnExpression = returnExpression;

      public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
      {
         var flags = ExpressionFlags.Standard;
         if (returnExpression)
            flags |= ExpressionFlags.OmitSendMessageAssign;
         if (getExpression(state, flags).If(out var expression, out var original))
         {
            state.AddStatement(new ExpressionStatement(expression, returnExpression));
            return Unit.Matched();
         }
         else
            return original.Unmatched<Unit>();
      }

      public override bool UpdateIndexOnParseOnly => true;
   }
}