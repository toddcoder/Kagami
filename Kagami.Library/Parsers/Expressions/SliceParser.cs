using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class SliceParser : SymbolParser
	{
		public SliceParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /'$' -(> ['cdefgnprxs' dquote])";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Operator);

			if (getExpression(state, builder.Flags).Out(out var expression, out var original))
			{
				builder.Add(new GetSliceSymbol(expression));

				var result =
					from scanned in state.Scan("^ /(|s|) /'=' -(> '=')", Color.Whitespace, Color.Structure)
					from expression1 in getExpression(state, builder.Flags)
					select expression1;
				if (result.If(out var value, out var mbException))
					builder.Add(new SetSliceSymbol(value));
				else if (mbException.If(out var exception))
					return failedMatch<Unit>(exception);
			}
			else
				return original.Unmatched<Unit>();

			return Unit.Matched();
		}
	}
}