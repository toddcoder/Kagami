using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class NewSelector : Operation
	{
		Selector selector;
		bool mutable;
		bool visible;

		public NewSelector(Selector selector, bool mutable, bool visible)
		{
			this.selector = selector;
			this.mutable = mutable;
			this.visible = visible;
		}

		public override IMatched<IObject> Execute(Machine machine)
		{
			if (machine.CurrentFrame.Fields.New(selector, mutable, visible).If(out _, out var exception))
				return notMatched<IObject>();
			else
				return failedMatch<IObject>(exception);
		}

		public override string ToString() => $"new.selector({selector}, {mutable}, {visible})";
	}
}