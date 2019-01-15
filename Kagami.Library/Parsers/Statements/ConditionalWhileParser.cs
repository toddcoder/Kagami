using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class ConditionalWhileParser : StatementParser
	{
		public override string Pattern => "^ /'while' /(|s+|)";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			state.BeginTransaction();

			state.Colorize(tokens, Color.Keyword, Color.Whitespace);

			var result =
				from comparisand in getExpression(state, ExpressionFlags.Comparisand)
				from scanned in state.Scan("^ /(|s|) /':='", Color.Whitespace, Color.Structure)
				from expression in getExpression(state, ExpressionFlags.Standard)
				from block in getBlock(state)
				select (comparisand, expression, block);

			if (result.Out(out var tuple, out var original))
			{
				state.CommitTransaction();
				var (comparisand, expression, block) = tuple;
				state.AddStatement(new ConditionalWhile(comparisand, expression, block));

				return Unit.Matched();
			}
			else
			{
				state.RollBackTransaction();
				return original.Unmatched<Unit>();
			}
		}
	}
}