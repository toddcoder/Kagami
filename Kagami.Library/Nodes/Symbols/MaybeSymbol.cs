using Kagami.Library.Operations;
using static Kagami.Library.Nodes.NodeFunctions;

namespace Kagami.Library.Nodes.Symbols
{
	public class MaybeSymbol : Symbol
	{
		public override void Generate(OperationsBuilder builder)
		{
			var isTrueLabel = newLabel("true");
			var endLabel = newLabel("end");
			builder.Swap();
			builder.GoToIfTrue(isTrueLabel);
			builder.Drop();
			builder.PushNil();
			builder.GoTo(endLabel);

			builder.Label(isTrueLabel);
			builder.Some();

			builder.Label(endLabel);
			builder.NoOp();

		}

		public override Precedence Precedence => Precedence.Boolean;

		public override Arity Arity => Arity.Binary;

		public override string ToString() => "maybe";
	}
}