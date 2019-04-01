using Kagami.Library.Nodes.Symbols;
using Core.Monads;

namespace Kagami.Library.Parsers.Expressions
{
	public class TryParser : EndingInExpressionParser
	{
		public TryParser(ExpressionBuilder builder, ExpressionFlags flags = ExpressionFlags.Standard) : base(builder, flags) { }

		public override string Pattern => "^ /(|s|) /'try' /b";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Keyword);
			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			builder.Add(new TrySymbol(expression));
			return Unit.Matched();
		}
	}
}