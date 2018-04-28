using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class NotSymbol : Symbol
	{
		public override void Generate(OperationsBuilder builder)
		{
			builder.PushBoolean(false);
			builder.Equal();
		}

		public override Precedence Precedence => Precedence.PrefixOperator;

		public override Arity Arity => Arity.Prefix;

		public override string ToString() => "not";
	}
}