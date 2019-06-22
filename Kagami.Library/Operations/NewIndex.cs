using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations
{
	public class NewIndex : TwoOperandOperation
	{
		public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
		{
			if (x is Int xInt)
			{
				if (y is Int yInt)
				{
					return Index.New(xInt.Value, yInt.Value).Matched();
				}
				else
				{
					return failedMatch<IObject>(incompatibleClasses(y, "Int"));
				}
			}
			else
			{
				return failedMatch<IObject>(incompatibleClasses(x, "Int"));
			}
		}

		public override string ToString() => "new.index";
	}
}