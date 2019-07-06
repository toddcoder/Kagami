using System.Linq;
using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations
{
	public class NewDictionary : OneOperandOperation
	{
		public override IMatched<IObject> Execute(Machine machine, IObject value)
		{
			switch (value)
			{
				case Container list:
					return Dictionary.New(list.List.ToArray()).Matched();
				case IKeyValue _:
					return Dictionary.New(new[] { value }).Matched();
				case ICollection collection when collection.ExpandForArray:
					return Dictionary.New(collection.GetIterator(false).List().ToArray()).Matched();
				case IIterator iterator:
					return Dictionary.New(iterator.List().ToArray()).Matched();
				default:
					return $"Dictionary can't be created with {value}".FailedMatch<IObject>();
			}
        }
	}
}