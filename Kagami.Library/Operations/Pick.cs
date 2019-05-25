using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class Pick : Operation
	{
		int index;

		public Pick(int index) => this.index = index;

		public override IMatched<IObject> Execute(Machine machine)
		{
			return machine.CurrentFrame.Pick(index).FlatMap(i => i.Matched(), notMatched<IObject>);
		}

		public override string ToString() => $"pick({index})";
	}
}