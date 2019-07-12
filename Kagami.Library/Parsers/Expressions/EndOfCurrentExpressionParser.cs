using Core.Monads;
using Kagami.Library.Nodes.Symbols;

namespace Kagami.Library.Parsers.Expressions
{
	public class EndOfCurrentExpressionParser : EndingInExpressionParser
	{
		public EndOfCurrentExpressionParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /';'";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Structure);
			if (builder.ToExpression().If(out var expression))
			{
				builder = new ExpressionBuilder(builder.Flags);
				builder.Add(new SubexpressionSymbol(expression));
			}

			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			builder.Add(expression);
			return Unit.Matched();
		}
	}
}