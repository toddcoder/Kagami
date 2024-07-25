using Kagami.Library.Nodes.Symbols;
using Core.Monads;

namespace Kagami.Library.Parsers.Expressions
{
	public class DollarFieldParser : SymbolParser
	{
		public DollarFieldParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /'$' /(/d+) /b";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var index = tokens[3].Text;
			state.Colorize(tokens, Color.Whitespace, Color.Identifier, Color.Identifier);

			builder.Add(new FieldSymbol($"__${index}"));

			return Unit.Matched();
		}
	}
}