using Kagami.Library.Nodes.Symbols;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Parsers.Expressions
{
	public class ForExpressionParser : EndingInExpressionParser
	{
		public ForExpressionParser(ExpressionBuilder builder, ExpressionFlags flags = ExpressionFlags.Standard) :
			base(builder, flags) { }

		public override string Pattern => "^ /(|s|) /'%' -(> '>')";

		public override IMatched<Unit> Prefix(ParseState state, Token[] tokens)
		{
			state.Colorize(tokens, Color.Whitespace, Color.Structure);
			return Unit.Matched();
		}

		public override IMatched<Unit> Suffix(ParseState state, Expression expression)
		{
			var implicitExpressionState = state.ImplicitExpressionState;
			if (implicitExpressionState.Symbol1.IsNone)
			{
				implicitExpressionState.Symbol1 = expression.Some<Symbol>();
				builder.Add(new FieldSymbol(implicitExpressionState.FieldName1));

				return Unit.Matched();
			}
			else if (implicitExpressionState.Symbol2.IsNone)
			{
				implicitExpressionState.Symbol2 = expression.Some<Symbol>();
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