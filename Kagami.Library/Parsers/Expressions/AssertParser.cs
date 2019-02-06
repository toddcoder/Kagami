using Kagami.Library.Nodes.Symbols;
using Standard.Types.Monads;
using static Kagami.Library.Parsers.ParserFunctions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class AssertParser : SymbolParser
	{
		public AssertParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /('assert' | 'some') /b";

		public override IMatched<Unit> Parse(ParseState state, Token[] tokens, ExpressionBuilder builder)
		{
			var isSuccess = tokens[2].Text == "assert";
			state.Colorize(tokens, Color.Whitespace, Color.Keyword);

			var result =
				from condition in getExpression(state, builder.Flags | ExpressionFlags.OmitColon)
				from colon1 in state.Scan("^ /(|s|) /':'", Color.Whitespace, Color.Structure)
				from value in getExpression(state, builder.Flags | ExpressionFlags.OmitColon)
				select (condition, value);

			if (isSuccess)
			{
				if (result.Out(out var tuple, out var original))
				{
					var (condition, value) = tuple;

					var result2 =
						from colon2 in state.Scan("^ /(|s|) /':'", Color.Whitespace, Color.Structure)
						from error in getExpression(state, builder.Flags | ExpressionFlags.OmitColon)
						select error;
					if (result2.Out(out var expression, out var original2))
					{
						builder.Add(new AssertSymbol(condition, value, expression.Some()));
						return Unit.Matched();
					}
					else
						return original2.Unmatched<Unit>();
				}
				else
					return original.Unmatched<Unit>();
			}
			else if (result.Out(out var tuple, out var original))
			{
				var (condition, value) = tuple;
				builder.Add(new AssertSymbol(condition, value, none<Expression>()));

				return Unit.Matched();
			}
			else
				return original.Unmatched<Unit>();
		}
	}
}