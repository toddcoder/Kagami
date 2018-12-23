using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class SeqParser : SymbolParser
	{
		public SeqParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(|s|) /'seq' /({REGEX_EOL})";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);
			state.CreateYieldFlag();

			if (getBlock(state).Out(out var block, out var original))
			{
				var yielding = state.RemoveYieldFlag();
				if (yielding)
				{
					builder.Add(new SeqSymbol(block));
					return Unit.Matched();
				}
				else
					return "Yield required".FailedMatch<Unit>();
			}
			else
				return original.UnmatchedOnly<Unit>();
		}
	}
}