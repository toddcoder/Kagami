using System.Linq;
using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations
{
	public class NewSet : OneOperandOperation
	{
		public override IMatched<IObject> Execute(Machine machine, IObject value)
		{
			switch (value)
			{
				case InternalList list:
					return new Set(list.List.ToArray()).Matched<IObject>();
				case ICollection collection when collection.ExpandForArray:
					return new Set(collection.GetIterator(false).List().ToArray()).Matched<IObject>();
				case IIterator iterator:
					return new Set(iterator.List().ToArray()).Matched<IObject>();
				default:
					return new Set(value).Matched<IObject>();
			}
		}

		public override string ToString() => "new.set";
	}
}