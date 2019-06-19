using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
	public class CycleIterator : Iterator
	{
		Cycle cycle;
		IMaybe<IObject> lastItem;
		IMaybe<(IObject, Lambda)> anySeedLambda;

		public CycleIterator(ICollection collection) : base(collection)
		{
			cycle = (Cycle)collection;
			lastItem = none<IObject>();
			anySeedLambda = cycle.SeedLambda;
		}

		public override IMaybe<IObject> Next()
		{
			if (anySeedLambda.If(out var seedLambda))
			{
				var (seed, lambda) = seedLambda;
				if (lastItem.If(out var item))
				{
					item = lambda.Invoke(item);
					lastItem = item.Some();
				}
				else
					lastItem = seed.Some();
			}
			else
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
			}

			return lastItem;
		}
	}
}