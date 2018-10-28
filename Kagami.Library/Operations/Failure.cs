using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
	public class Failure : OneOperandOperation
	{
		public override IMatched<IObject> Execute(Machine machine, IObject value) => Objects.Failure.Object(value.AsString).Matched();

		public override string ToString() => "failure";
	}
}