using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
	public class DoSymbol : Symbol
	{
		MonadBindSymbol[] monadBindSymbols;
		Expression expression;

		public DoSymbol(MonadBindSymbol[] monadBindSymbols, Expression expression)
		{
			this.monadBindSymbols = monadBindSymbols;
			this.expression = expression;
		}

		public override void Generate(OperationsBuilder builder)
		{
			var failureLabel = newLabel("failure");

			builder.PushFrame();

			foreach (var symbol in monadBindSymbols)
			{
				symbol.FailureLabel = failureLabel;
				symbol.Generate(builder);
			}

			expression.Generate(builder);
			builder.Success();

			builder.Label(failureLabel);
			builder.NoOp();

			builder.PopFrameWithValue();
		}

		public override Precedence Precedence => Precedence.Value;

		public override Arity Arity => Arity.Nullary;
	}
}