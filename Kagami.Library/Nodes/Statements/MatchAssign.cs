using Kagami.Library.Nodes.Symbols;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
	public class MatchAssign : Statement
	{
		Expression comparisand;
		Expression expression;

		public MatchAssign(Expression comparisand, Expression expression)
		{
			this.comparisand = comparisand;
			this.expression = expression;
		}

		public override void Generate(OperationsBuilder builder)
		{
			expression.Generate(builder);
			builder.Peek(Index);
			comparisand.Generate(builder);
			builder.Match();
			builder.Drop();
      }

		public override string ToString() => $"{comparisand} := {expression}";
	}
}