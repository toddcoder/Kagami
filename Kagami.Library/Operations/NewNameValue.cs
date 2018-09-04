using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
	public class NewNameValue : TwoOperandOperation
	{
		public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
		{
			if (x is String s)
				return new NameValue(s.Value, y).Matched<IObject>();
			else
				return failedMatch<IObject>(incompatibleClasses(x, "String"));
		}

		public override string ToString() => "new.name.value";
	}
}