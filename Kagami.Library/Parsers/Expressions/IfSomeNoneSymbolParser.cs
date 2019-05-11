using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class IfSomeNoneSymbolParser : SymbolParser
	{
		public IfSomeNoneSymbolParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /'||'";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.BeginTransaction();

			state.Colorize(tokens, Color.Whitespace, Color.Structure);

			if (getExpression(state, builder.Flags).If(out var expression, out var mbException))
			{
				state.CommitTransaction();
				builder.Add(new IfSomeNoneSymbol(expression));

				return Unit.Matched();
			}
			else if (mbException.If(out var exception))
				return failedMatch<Unit>(exception);
			else
			{
				state.RollBackTransaction();
				return notMatched<Unit>();
			}
		}
	}
}