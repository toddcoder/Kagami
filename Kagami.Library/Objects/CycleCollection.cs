using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Objects
{
	public class CycleCollection : IObject, ICollection
	{
		Cycle cycle;
		IMaybe<IObject> lastItem;
		IMaybe<(IObject, Lambda)> anySeedLambda;

		public CycleCollection(Cycle cycle)
		{
			this.cycle = cycle;
			lastItem = none<IObject>();
			anySeedLambda = cycle.SeedLambda;
		}

		public string ClassName => cycle.ClassName;

		public string AsString => cycle.AsString;

		public string Image => cycle.Image;

		public int Hash => cycle.Hash;

		public bool IsEqualTo(IObject obj) => cycle.IsEqualTo(obj);

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => cycle.Match(comparisand, bindings);

		public bool IsTrue => cycle.IsTrue;

		public IIterator GetIterator(bool lazy) => new LazyIterator(this);

		public IMaybe<IObject> Next(int index)
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
				{
					lastItem = seed.Some();
				}
			}
			else
			{
				var item = cycle[index++ % cycle.Length.Value];
				if (item is Lambda lambda)
				{
					var parameterCount = lambda.Invokable.Parameters.Length;
					if (parameterCount > 0 && lastItem.If(out var value))
					{
						item = lambda.Invoke(value);
					}
					else
					{
						item = lambda.Invoke();
					}

					lastItem = item.Some();
				}
				else
				{
					lastItem = item.Some();
				}
			}

			return lastItem;
        }

		public IMaybe<IObject> Peek(int index) => lastItem;

		public Int Length => cycle.Length;

		public bool ExpandForArray => cycle.ExpandForArray;

		public Boolean In(IObject item) => cycle.In(item);

		public Boolean NotIn(IObject item) => cycle.NotIn(item);

		public IObject Times(int count) => cycle.Times(count);

		public String MakeString(string connector) => cycle.MakeString(connector);

		public IIterator GetIndexedIterator() => new IndexedIterator(this);
	}
}