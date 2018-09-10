using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Symbols
{
	public class SkipTakeRestInitSymbol : SkipTakeRestSymbol
	{
		public override void Generate(OperationsBuilder builder)
		{
			builder.NewSkipTake();
			base.Generate(builder);
		}
	}
}