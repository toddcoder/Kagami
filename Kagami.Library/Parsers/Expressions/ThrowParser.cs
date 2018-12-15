using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
	public class ThrowParser : EndingInExpressionParser
	{
		public ThrowParser(ExpressionBuilder builder, ExpressionFlags flags = ExpressionFlags.Standard) : base(builder, flags) { }

		public override string Pattern => "^ /(|s|) /'throw' /b";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Keyword);
			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			builder.Add(new ThrowSymbol(expression));
			return Unit.Matched();
		}
	}
}