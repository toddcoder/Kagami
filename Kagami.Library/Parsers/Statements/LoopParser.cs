using Kagami.Library.Nodes.Statements;
using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class LoopParser : StatementParser
	{
		public override string Pattern => $"^ /'loop' {REGEX_ANTICIPATE_END}";

		static IMatched<Expression> getUntil(ParseState state)
		{
			var untilParser = new UntilParser();
			return untilParser.Scan(state).Map(u => untilParser.Expression);
		}

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			state.BeginTransaction();

			state.Colorize(tokens, Color.Keyword);

			var result =
				from skipped in state.SkipEndOfLine()
				from b in getBlock(state)
				from e in getUntil(state)
				select (b, e);

			if (result.Out(out var tuple, out var original))
			{
				var (block, expression) = tuple;
				state.AddStatement(new Loop(block, expression));
				state.CommitTransaction();

				return Unit.Matched();
			}
			else
			{
				state.RollBackTransaction();
				return original.UnmatchedOnly<Unit>();
			}
		}
	}
}