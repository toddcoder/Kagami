using Core.Monads;
using Kagami.Library.Nodes.Symbols;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class ImplicitSymbolParser : EndingInValueParser
	{
		public ImplicitSymbolParser(ExpressionBuilder builder) : base(builder) { }

		public override string Pattern => "^ /(|s|) /'^' -(> /s)";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Structure);
			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Symbol value)
		{
			var implicitExpressionState = state.ImplicitExpressionState;
			if (implicitExpressionState.Symbol1.IsNone)
			{
				implicitExpressionState.Symbol1 = value.Some();
				builder.Add(new FieldSymbol(implicitExpressionState.FieldName1));

				return Unit.Matched();
			}
			else if (implicitExpressionState.Symbol2.IsNone)
			{
				implicitExpressionState.Symbol2 = value.Some();
				builder.Add(new FieldSymbol(implicitExpressionState.FieldName2));

				return Unit.Matched();
			}
			else
			{
				return notMatched<Unit>();
			}
		}
	}
}