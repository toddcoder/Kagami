using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
	public class Throw : OneOperandOperation
	{
		public override IMatched<IObject> Execute(Machine machine, IObject value)
		{
			var errorMessage = value.AsString;
			return errorMessage.FailedMatch<IObject>();
		}

		public override string ToString() => "throw";
	}
}