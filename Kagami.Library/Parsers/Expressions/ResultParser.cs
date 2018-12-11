using Kagami.Library.Nodes.Symbols;
using Standard.Types.Maybe;

namespace Kagami.Library.Parsers.Expressions
{
	public class ResultParser : EndingInExpressionParser
	{
		public ResultParser(ExpressionBuilder builder, ExpressionFlags flags = ExpressionFlags.Standard) : base(builder, flags) { }

		public override string Pattern => "^ /(|s|) /'result' /(|s+|)";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Keyword, Color.Whitespace);
			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			Expression = expression;
			return Unit.Matched();
		}

		public Expression Expression { get; set; }
	}
}