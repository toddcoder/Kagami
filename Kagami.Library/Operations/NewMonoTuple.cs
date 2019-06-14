using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations
{
	public class NewMonoTuple : OneOperandOperation
	{
		public override IMatched<IObject> Execute(Machine machine, IObject value) => new Tuple(value).Matched<IObject>();

		public override string ToString() => "new.mono.tuple";
	}
}