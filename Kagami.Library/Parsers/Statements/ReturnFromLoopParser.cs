using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class ReturnFromLoopParser : StatementParser
	{
		public override string Pattern => "^ /'return' /b";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			state.BeginTransaction();
			state.Colorize(tokens, Color.Keyword);

			var result =
				from expression in getExpression(state, ExpressionFlags.OmitIf)
				from @if in state.Scan("^ /(|s|) /'if' /b", Color.Whitespace, Color.Keyword)
				from condition in getExpression(state, ExpressionFlags.Standard)
				select (expression, condition);

			if (result.If(out var tuple, out var original))
			{
				var (expression, condition) = tuple;
				Condition = condition;
				Expression = expression;

				state.CommitTransaction();

				return Unit.Matched();
			}
			else
			{
				state.RollBackTransaction();
				return original.UnmatchedOnly<Unit>();
			}
		}

		public Expression Condition { get; set; }

		public Expression Expression { get; set; }
	}
}