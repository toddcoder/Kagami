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
			var createNewField = false;
			foreach (var subSelector in selector.AllSelectors())
			{
				if (createNewField)
					if (machine.CurrentFrame.Fields.New(subSelector, overriding).If(out _, out var anyException)) { }
					else
						return failedMatch<IObject>(anyException);

				if (machine.Assign(subSelector, value, overriding).If(out _, out var exception))
					createNewField = true;
				else
					return failedMatch<IObject>(exception);
			}

			return notMatched<IObject>();
		}

		public override string ToString() => $"assign.selector({selector.Image}, {overriding})";
	}
}