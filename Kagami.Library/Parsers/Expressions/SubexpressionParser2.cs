using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class SubexpressionParser2 : SymbolParser
	{
		public SubexpressionParser2(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /'(' /','?";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.BeginTransaction();
			var monoTuple = tokens[3].Text == ",";
			state.Colorize(tokens, Color.Whitespace, Color.OpenParenthesis, Color.Structure);

			if (getExpression(state, "^ /')'", builder.Flags & ~ExpressionFlags.OmitComma, Color.CloseParenthesis)
				.If(out var expression, out var mbException))
			{
				builder.Add(new SubexpressionSymbol(expression, monoTuple));
				state.CommitTransaction();

				return Unit.Matched();
			}
			else if (mbException.IsSome)
			{
				state.RollBackTransaction();
				state.BeginTransaction();
				if (getPartialLambda(state).Out(out var lambdaSymbol, out var original))
				{
					state.CommitTransaction();
					builder.Add(lambdaSymbol);

					return Unit.Matched();
				}
				else
				{
					state.RollBackTransaction();
					return original.Unmatched<Unit>();
				}
			}
			else
				return notMatched<Unit>();
		}
	}
}