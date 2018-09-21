using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class DoesSymbol : Symbol
	{
		Expression expression;

		public DoesSymbol(Expression expression) => this.expression = expression;

		public override void Generate(OperationsBuilder builder)
		{
			expression.Generate(builder);
			builder.SendMessage("respondTo", 1);
		}

		public override Precedence Precedence => Precedence.Boolean;

		public override Arity Arity => Arity.Binary;

		public override string ToString() => $"does {expression}";
	}
}