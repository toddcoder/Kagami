using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
	public class RangeOperatorParser : SymbolParser
	{
		public override string Pattern => "^ /(|s|) /('..' ['.<'])";

		public RangeOperatorParser(ExpressionBuilder builder) : base(builder) { }

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var inclusive = tokens[2].Text == "...";
			state.Colorize(tokens, Color.Whitespace, Color.Operator);

			builder.Add(new RangeSymbol(inclusive));
			return Unit.Matched();
		}
	}
}