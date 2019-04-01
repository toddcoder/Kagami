using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class ZipOperatorParser : SymbolParser
	{
		public ZipOperatorParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(|s|) /'[|' /({REGEX_OPERATORS}1%2) /'|]'";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var source = tokens[3].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Operator, Color.Operator, Color.Operator);

			if (getOperator(state, source, builder.Flags, true).Out(out var symbol, out var original))
			{
				builder.Add(new ZipOperatorSymbol(symbol));
				return Unit.Matched();
			}
			else
				return original.UnmatchedOnly<Unit>();
		}
	}
}