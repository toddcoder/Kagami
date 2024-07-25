using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class RepeatParser : StatementParser
	{
		public override string Pattern => "^ /'repeat' /b";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Keyword);

			var result =
				from expression in getExpression(state, ExpressionFlags.Standard)
				from scanned in state.Scan($"^ /(|s+|) /'times' {REGEX_ANTICIPATE_END}", Color.Whitespace, Color.Keyword)
				from block in getBlock(state)
				select (expression, block);

			if (result.If(out var tuple))
			{
				var (expression, block) = tuple;
				state.AddStatement(new Repeat(expression, block));

				return Unit.Matched();
			}
			else
			{
				return result.UnmatchedOnly<Unit>();
			}
		}
	}
}