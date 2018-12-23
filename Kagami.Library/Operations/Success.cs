using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
	public class Success : OneOperandOperation
	{
		public override IMatched<IObject> Execute(Machine machine, IObject value) => Objects.Success.Object(value).Matched();

		public override string ToString() => "success";
	}
}