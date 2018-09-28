using Kagami.Library.Objects;
using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class MutStringSymbol : Symbol
	{
		MutString mutString;

		public MutStringSymbol(MutString mutString) => this.mutString = mutString;

		public override void Generate(OperationsBuilder builder) => builder.PushObject(mutString);

		public override Precedence Precedence => Precedence.Value;

		public override Arity Arity => Arity.Nullary;

		public override string ToString() => mutString.Image;
	}
}