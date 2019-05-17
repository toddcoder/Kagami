using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Kagami.Library.Parsers.ParserFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class CycleParser : SymbolParser
	{
		public CycleParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(/s*) /'.(' /(/s*)";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Collection, Color.Whitespace);
			if (getExpression(state, "^ /(/s*) /')'", builder.Flags, Color.Whitespace, Color.Collection)
				.Out(out var expression, out var original))
			{
				builder.Add(new CycleSymbol(expression));
				return Unit.Matched();
			}
			else
				return original.UnmatchedOnly<Unit>();
		}
	}
}