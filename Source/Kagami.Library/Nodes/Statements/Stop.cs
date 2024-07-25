using Kagami.Library.Operations;

namespace Kagami.Library.Nodes.Statements
{
	public class Stop : Statement
	{
		public override void Generate(OperationsBuilder builder) => builder.Stop();

		public override string ToString() => "stop";
	}
}