using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
	public class TrySymbol : Symbol
	{
		Expression expression;

		public TrySymbol(Expression expression) => this.expression = expression;

		public override void Generate(OperationsBuilder builder)
		{
			var errorLabel = newLabel("error");

			builder.TryBegin();
			builder.SetErrorHandler(errorLabel);
			expression.Generate(builder);
			builder.TryEnd();

			builder.Label(errorLabel);
			builder.NoOp();
		}

		public override Precedence Precedence => Precedence.Value;

		public override Arity Arity => Arity.Nullary;

		public override string ToString() => $"try {expression}";
	}
}