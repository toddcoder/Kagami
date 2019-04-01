using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public abstract class EndingInExpressionParser : StatementParser
	{
		public abstract IMatched<Unit> Prefix(ParseState state, Token[] tokens);

		public abstract IMatched<Unit> Suffix(ParseState state, Expression expression);

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens) =>
			from prefix in Prefix(state, tokens)
			from expression in getExpression(state, ExpressionFlags.Standard)
			from suffix in Suffix(state, expression)
			select suffix;
	}
}