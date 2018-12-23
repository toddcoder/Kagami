using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
	public class ArgumentLabel : OneOperandOperation
	{
		string label;

		public ArgumentLabel(string label) => this.label = label;

		public override IMatched<IObject> Execute(Machine machine, IObject value) => new NameValue(label, value).Matched<IObject>();

		public override string ToString() => $"argument.label({label})";
	}
}