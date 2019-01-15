using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class MatchAssignParser : StatementParser
	{
		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			state.BeginTransaction();
			var result =
				from comparisand in getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitColon)
				from stem in state.Scan("^ /(|s|) /':='", Color.Whitespace, Color.Structure)
				from expression in getExpression(state, ExpressionFlags.Standard)
				select (comparisand, expression);
			if (result.If(out var tuple, out var mbException))
			{
				state.CommitTransaction();
				var (comparisand, expression) = tuple;
				state.AddStatement(new MatchAssign(comparisand, expression));

				return Unit.Matched();
			}
			else if (mbException.If(out var exception))
			{
				state.RollBackTransaction();
				return failedMatch<Unit>(exception);
			}
			else
			{
				state.RollBackTransaction();
				return notMatched<Unit>();
			}
		}

/*		static IMatched<Statement> getRestOfStatement(ParseState state)
		{

		}*/
	}
}