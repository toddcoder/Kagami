using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class MonadBindSymbol : Symbol
	{
		string fieldName;
		Expression expression;

		public MonadBindSymbol(string fieldName, Expression expression)
		{
			this.fieldName = fieldName;
			this.expression = expression;
		}

		public override void Generate(OperationsBuilder builder)
		{
			expression.Generate(builder);
			builder.GoToIfFailure(FailureLabel);
			builder.NewField(fieldName, false, true);
			builder.SendMessage("__$value");
			builder.AssignField(fieldName, true);
		}

		public override Precedence Precedence => Precedence.KeyValue;

		public override Arity Arity => Arity.Binary;

		public override string ToString() => $"{fieldName} := {expression}";

		public string FailureLabel { get; set; } = "";
	}
}