using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
	public class NewSkipTake : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => notMatched<IObject>();//new SkipTake(value).Matched<IObject>();

		public override string ToString() => "new.skip.take";
	}
}