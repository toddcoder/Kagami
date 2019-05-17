using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
	public struct Index : IObject
	{
		public static IObject New(int skipCount, int takeCount) => new Index(skipCount, takeCount);

		int skipCount;
		int takeCount;

		public Index(int skipCount, int takeCount) : this()
		{
			this.skipCount = skipCount;
			this.takeCount = takeCount;
		}

		public string ClassName => "Index";

		public string AsString => $"{skipCount};{takeCount}";

		public string Image => AsString;

		public int Hash => (skipCount + takeCount).GetHashCode();

		public bool IsEqualTo(IObject obj) => obj is Index index && skipCount == index.skipCount && takeCount == index.takeCount;

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

		public bool IsTrue => skipCount != 0 || takeCount != 0;

		public IObject IndexOf(ICollection collection)
		{
			var iterator = collection.GetIterator(false);
			var stage1 = (ICollection)iterator.Skip(skipCount);

			if (takeCount == 0)
				return (IObject)stage1;
			else
			{
				iterator = stage1.GetIterator(false);
				return iterator.Take(takeCount);
			}
		}

		public Int SkipCount => skipCount;

		public Int TakeCount => takeCount;
	}
}