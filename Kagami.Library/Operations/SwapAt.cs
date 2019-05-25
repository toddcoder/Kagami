using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class SwapAt : Operation
	{
		int index;

		public SwapAt(int index) => this.index = index;

		public override IMatched<IObject> Execute(Machine machine)
		{
			return machine.CurrentFrame.Swap(index)
				.FlatMap(u => notMatched<IObject>(), () => "Swap at out of range".FailedMatch<IObject>());
		}

		public override string ToString() => $"swap.at({index})";
	}
}