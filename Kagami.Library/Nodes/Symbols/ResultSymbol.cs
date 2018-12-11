using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
	public class ResultSymbol : Symbol
	{
		Expression expression;

		public ResultSymbol(Expression expression) => this.expression = expression;

		public override void Generate(OperationsBuilder builder)
		{
			var successLabel = newLabel("success");
			var endLabel = newLabel("end");

			builder.GoToIfSuccess(successLabel);
			builder.SendMessage("error".get());
			builder.Failure();
			builder.GoTo(endLabel);

			builder.Label(successLabel);
			expression.Generate(builder);
			builder.Success();

			builder.Label(endLabel);
			builder.NoOp();
		}

		public override Precedence Precedence => Precedence.ChainedOperator;

		public override Arity Arity => Arity.Prefix;

		public override string ToString() => $"result {expression}";

		public string FailureLabel { get; set; } = "";
	}
}