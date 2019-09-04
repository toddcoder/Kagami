using Core.Monads;
using Kagami.Library.Objects;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Mixins
{
	public class Collecting : Mixin, ICollection
	{
		public Collecting() : base("Collecting") { }

		public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

		public IMaybe<IObject> Next(int index) => none<IObject>();

		public IMaybe<IObject> Peek(int index) => none<IObject>();

		public Int Length => 0;

		public bool ExpandForArray => false;

		public Boolean In(IObject item) => false;

		public Boolean NotIn(IObject item) => false;

		public IObject Times(int count) => this;

		public String MakeString(string connector) => "";

		public IIterator GetIndexedIterator() => new IndexedIterator(this);
	}
}