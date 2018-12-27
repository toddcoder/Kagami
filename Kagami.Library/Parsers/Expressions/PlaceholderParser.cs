using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class PlaceholderParser : SymbolParser
	{
		public PlaceholderParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => $"^ /(|s|) /('existing' | 'var') /(|s+|) /({REGEX_FIELD}) /b";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var mutable = tokens[2].Text;
			var placeholderName = tokens[4].Text;
			var name = "";
			switch (mutable)
			{
            case "existing":
	            name = placeholderName;
					break;
            case "var":
	            name = $"+{placeholderName}";
					break;
				default:
					name = $"-{placeholderName}";
					break;
			}
			state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace, Color.Identifier);

			builder.Add(new PlaceholderSymbol(name));

			return Unit.Matched();
		}
	}
}