using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
	public abstract class ValueParser : SymbolParser
	{
		protected bool isComparisand;

		protected ValueParser(ExpressionBuilder builder, bool isComparisand) : base(builder) => this.isComparisand = isComparisand;

		public abstract IMatched<Unit> ParseValue(ParseState state, Token[] tokens, ExpressionBuilder builder);

		public abstract IMatched<Unit> ParseComparisand(ParseState state, Token[] tokens, ExpressionBuilder builder);

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			if (isComparisand)
				return ParseComparisand(state, tokens, builder);

			return ParseValue(state, tokens, builder);
		}
	}
}