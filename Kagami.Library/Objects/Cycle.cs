using System.Collections.Generic;
using System.Linq;
using Core.Arrays;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Kagami.Library.Objects.CollectionFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
	public class Cycle : IObject, ICollection
	{
		public static IObject CreateObject(IEnumerable<IObject> items) => new Cycle(items.ToArray());

		IObject[] items;

		public Cycle(params IObject[] items) => this.items = items;

		public string ClassName => "Cycle";

		public string AsString => $".({items.Select(i => i.AsString).Join()})";

		public string Image => $".({items.Select(i => i.Image).Join()})";

		public int Hash => items.GetHashCode();

		public bool IsEqualTo(IObject obj) => isEqualTo(this, obj);

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

		public bool IsTrue => items.Length > 0;

		public IIterator GetIterator(bool lazy) => lazy ? new LazyIterator(this) : new Iterator(this);

		public IMaybe<IObject> Next(int index) => items[index % items.Length].Some();

		public IMaybe<IObject> Peek(int index) => Next(index);

		public Int Length => items.Length;

		public bool ExpandForArray => true;

		public Boolean In(IObject item) => items.Contains(item);

		public Boolean NotIn(IObject item) => !items.Contains(item);

		public IObject Times(int count) => new Cycle(items.Repeat(count));

		public String MakeString(string connector) => makeString(this, connector);

		public IIterator GetIndexedIterator() => new IndexedIterator(this);

		public Tuple Items => new Tuple(items);
	}
}