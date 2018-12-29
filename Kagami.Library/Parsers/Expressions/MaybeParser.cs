using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;

namespace Kagami.Library.Parsers.Expressions
{
	public class MaybeParser : EndingInExpressionParser
	{
		public MaybeParser(ExpressionBuilder builder, ExpressionFlags flags = ExpressionFlags.Standard) : base(builder, flags) { }

		public override string Pattern => "^ /(|s|) /'maybe' /b";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Operator);
			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			builder.Add(new MaybeSymbol(expression));
			return Unit.Matched();
		}
	}
}