using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Mixins
{
	public class CollectionMixin : Mixin, ICollection
	{
		public CollectionMixin(string name) : base(name) { }

		public IIterator GetIterator(bool lazy) => TODO_IMPLEMENT_ME;

		public IMaybe<IObject> Next(int index) => TODO_IMPLEMENT_ME;

		public IMaybe<IObject> Peek(int index) => TODO_IMPLEMENT_ME;

		public Int Length { get; }

		public bool ExpandForArray { get; }

		public Boolean In(IObject item) => TODO_IMPLEMENT_ME;

		public Boolean NotIn(IObject item) => TODO_IMPLEMENT_ME;

		public IObject Times(int count) => TODO_IMPLEMENT_ME;

		public String MakeString(string connector) => TODO_IMPLEMENT_ME;

		public IIterator GetIndexedIterator() => TODO_IMPLEMENT_ME;
	}
}