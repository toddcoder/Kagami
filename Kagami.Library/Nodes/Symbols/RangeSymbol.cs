using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class RangeSymbol : Symbol
	{
		bool inclusive;

		public RangeSymbol(bool inclusive) => this.inclusive = inclusive;

		public override void Generate(OperationsBuilder builder) => builder.NewRange(inclusive);

		public override Precedence Precedence => Precedence.Range;

		public override Arity Arity => Arity.Binary;

		public override string ToString() => inclusive ? "..." : "..<";
	}
}