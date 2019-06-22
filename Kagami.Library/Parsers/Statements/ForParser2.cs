using Kagami.Library.Nodes.Statements;
using Kagami.Library.Parsers.Expressions;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Statements
{
	public class ForParser2 : StatementParser
	{
		public override string Pattern => "^ /'for' /(|s+|)";

		public override IMatched<Unit> ParseStatement(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Keyword, Color.Whitespace);

			var result =
				from comparisand in getExpression(state, ExpressionFlags.Comparisand | ExpressionFlags.OmitColon)
				from scanned in state.Scan("^ /(|s|) /':='", Color.Whitespace, Color.Structure)
				from source in getExpression(state, ExpressionFlags.Standard)
				from block in getBlock(state)
				select (comparisand, source, block);
			if (result.Out(out var tuple, out var original))
			{
				var (comparisand, source, block) = tuple;
				state.AddStatement(new For2(comparisand, source, block));

				return Unit.Matched();
			}
			else
			{
				return original.Unmatched<Unit>();
			}
		}
	}
}