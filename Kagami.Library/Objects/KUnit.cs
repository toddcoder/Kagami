using Standard.Types.Collections;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Objects
{
	public struct KUnit : IObject
	{
		public static IObject Value = new KUnit();

		public string ClassName => "Unit";

		public string AsString => "";

		public string Image => "unit";

		public int Hash => "unit".GetHashCode();

		public bool IsEqualTo(IObject obj) => obj is KUnit;

		public bool Match(IObject comparisand, Hash<string, IObject> bindings) => match(this, comparisand, bindings);

		public bool IsTrue => false;
	}
}