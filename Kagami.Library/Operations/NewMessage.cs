using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
	public class NewMessage : OneOperandOperation
	{
		Selector selector;

		public NewMessage(Selector selector) => this.selector = selector;

		public override IMatched<IObject> Execute(Machine machine, IObject value)
		{
			if (value is Arguments arguments)
				return new Message(selector, arguments).Matched<IObject>();
			else
				return failedMatch<IObject>(incompatibleClasses(value, "Arguments"));
		}

		public override string ToString() => $"new.message({selector.AsString})";
	}
}