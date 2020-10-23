using System.Collections.Generic;
using System.Linq;
using Core.Collections;
using Core.Enumerables;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
	public class Container : IObject
	{
		List<IObject> list;

		public Container(IObject x, IObject y) => list = new List<IObject> { x, y };

		public Container(IEnumerable<IObject> objects) => list = new List<IObject>(objects);

		public List<IObject> List => list;

		public string ClassName => "Container";

		public string AsString => list.Select(i => i.AsString).ToString(" ");

		public string Image => list.Select(i => i.Image).ToString(", ");

		public int Hash => list.GetHashCode();

		public bool IsEqualTo(IObject obj)
		{
			return obj is Container container && list.Count == container.list.Count && list.All(i => list.Contains(i));
		}

		public bool Match(IObject comparisand, Hash<string, IObject> bindings)
		{
			return matchSingle(this, comparisand, (container, o) => container.In(o), bindings);
		}

		public bool IsTrue => list.Count > 0;

		public void Add(IObject item) => list.Add(item);

		public bool In(IObject obj) => list.Contains(obj);

		public bool ExpandInTuple { get; set; } = true;
	}
}