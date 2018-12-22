using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class PlaceholderParser : SymbolParser
	{
		public PlaceholderParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(|s|) /('let' | 'var') /(|s+|) /({REGEX_FIELD}) /b";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var mutable = tokens[2].Text == "var";
			var placeholderName = tokens[4].Text;
			var name = (mutable ? "+" : "-") + placeholderName;
			state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Identifier);

			builder.Add(new PlaceholderSymbol(name));

			return Unit.Matched();
		}
	}
}