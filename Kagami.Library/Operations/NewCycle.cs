using System.Linq;
using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations
{
	public class NewCycle : OneOperandOperation
	{
		public override IMatched<IObject> Execute(Machine machine, IObject value)
		{
			switch (value)
			{
				case Container list:
					return Cycle.CreateObject(list.List.ToArray()).Matched();
				case ICollection collection when collection.ExpandForArray:
					return Cycle.CreateObject(collection.GetIterator(false).List().ToArray()).Matched();
				case IIterator iterator:
					return Cycle.CreateObject(iterator.List().ToArray()).Matched();
				default:
					return new Cycle(value).Matched<IObject>();
			}
		}

		public override string ToString() => "new.cycle";
	}
}