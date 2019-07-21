using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
	public class MatchExpressionSymbol : Symbol
	{
		(Expression, Expression)[] matchItems;

		public MatchExpressionSymbol((Expression, Expression)[] matchItems) => this.matchItems = matchItems;

		public override void Generate(OperationsBuilder builder)
		{
			var nextLabel = newLabel("next_");
			var endLabel = newLabel("end");

			builder.PushFrameWithValue();

			for (var i = 0; i < matchItems.Length; i++)
			{
				var (comparisand, result) = matchItems[i];

				builder.Label(nextLabel + i);
				builder.Dup();
				comparisand.Generate(builder);
				builder.Match();
				builder.GoToIfFalse(nextLabel + (i + 1));

				result.Generate(builder);
				builder.GoTo(endLabel);
			}

			builder.Label(nextLabel + matchItems.Length);
			builder.PushString("No default value provided");
			builder.Throw();

			builder.Label(endLabel);
			builder.Swap();
			builder.Drop();

			builder.PopFrameWithValue();
		}

		public override Precedence Precedence => Precedence.Boolean;

		public override Arity Arity => Arity.Binary;
	}
}