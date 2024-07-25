using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class SpecialLessThanEqualSymbol : LessThanEqualSymbol
	{
		public override void Generate(OperationsBuilder builder)
		{
			builder.Pick(2);
			builder.Swap();
			base.Generate(builder);
			builder.And();
		}
	}
}