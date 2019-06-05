using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
	public class CycleIterator : Iterator
	{
		Cycle cycle;
		IMaybe<IObject> lastItem;

		public CycleIterator(ICollection collection) : base(collection)
		{
			cycle = (Cycle)collection;
			lastItem = none<IObject>();
		}

		public override IMaybe<IObject> Next()
		{
			var item = cycle[index++ % cycle.Length.Value];
			if (item is Lambda lambda)
			{
				var parameterCount = lambda.Invokable.Parameters.Length;
				if (parameterCount > 0 && lastItem.If(out var value))
					item = lambda.Invoke(value);
				else
					item = lambda.Invoke();
				lastItem = item.Some();
			}
			else
				lastItem = item.Some();

			return lastItem;
		}
	}
}