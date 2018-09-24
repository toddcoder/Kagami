using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Booleans;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
	public class AssignField : OneOperandOperation
	{
		string name;
		bool overriding;

		public AssignField(string name, bool overriding)
		{
			this.name = name;
			this.overriding = overriding;
		}

		public override IMatched<IObject> Execute(Machine machine, IObject value)
		{
			if (machine.Assign(name, value, false).If(out _, out var exception))
				return notMatched<IObject>();
			else
				return failedMatch<IObject>(exception);
		}

		public override string ToString() => $"assign.field({name}{overriding.Extend(", override")})";
	}
}