using Core.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
	public class Mixin : IObject
	{
		string name;

		public Mixin(string name) => this.name = name;

		public string ClassName => name;

		public string AsString => name;

		public string Image => $"mixin {name}";

		public int Hash => name.GetHashCode();

		public bool IsEqualTo(IObject obj) => obj is Mixin mixin && name == mixin.name;

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

		public bool IsTrue => true;
	}
}