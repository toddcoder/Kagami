using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
	public class NewSkipTake : OneOperandOperation
	{
		public override IMatched<IObject> Execute(Machine machine, IObject value) => new SkipTake(value).Matched<IObject>();

		public override string ToString() => "new.skip.take";
	}
}