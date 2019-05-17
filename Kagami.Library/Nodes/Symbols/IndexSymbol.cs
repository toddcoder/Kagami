using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class IndexSymbol : Symbol
	{
		public override void Generate(OperationsBuilder builder)
		{
			builder.NewIndex();
		}

		public override Precedence Precedence => Precedence.KeyValue;

		public override Arity Arity => Arity.Binary;

		public override string ToString() => ";";
	}
}