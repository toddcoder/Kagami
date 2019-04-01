using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class AssignSelector : OneOperandOperation
	{
		Selector selector;
		bool overriding;

		public AssignSelector(Selector selector, bool overriding)
		{
			this.selector = selector;
			this.overriding = overriding;
		}

		public override IMatched<IObject> Execute(Machine machine, IObject value)
		{
			if (machine.Assign(selector, value, overriding).If(out _, out var exception))
				return notMatched<IObject>();
			else
				return failedMatch<IObject>(exception);
		}

		public override string ToString() => $"assign.selector({selector.Image}, {overriding})";
	}
}