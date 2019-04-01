using Kagami.Library.Nodes.Statements;
using Kagami.Library.Objects;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class ExpressionStatementParser : StatementParser
	{
		bool returnExpression;
		IMaybe<TypeConstraint> typeConstraint;

		public ExpressionStatementParser(bool returnExpression, IMaybe<TypeConstraint> typeConstraint)
		{
			this.returnExpression = returnExpression;
			this.typeConstraint = typeConstraint;
		}

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			var flags = ExpressionFlags.Standard;
			if (returnExpression)
				flags |= ExpressionFlags.OmitSendMessageAssign;
			if (getExpression(state, flags).Out(out var expression, out var original))
			{
				state.AddStatement(new ExpressionStatement(expression, returnExpression, typeConstraint));
				return Unit.Matched();
			}
			else
				return original.Unmatched<Unit>();
		}

		public override bool UpdateIndexOnParseOnly => true;
	}
}