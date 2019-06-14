using System.Collections.Generic;
using System.Linq;
using Kagami.Library.Objects;

namespace Kagami.Library.Classes
{
	public class SetClass : BaseClass, ICollectionClass
	{
		public override string Name => "Set";

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			collectionMessages();

			messages["<<"] = (obj, msg) => ClassFunctions.function<Set, IObject>(obj, msg, (s, i) => s.Append(i));
			messages[">>"] = (obj, msg) => ClassFunctions.function<Set, IObject>(obj, msg, (s, i) => s.Remove(i));
			messages["+"] = (obj, msg) => ClassFunctions.function<Set, Set>(obj, msg, (s1, s2) => s1.Union(s2));
			messages["-"] = (obj, msg) => ClassFunctions.function<Set, Set>(obj, msg, (s1, s2) => s1.Difference(s2));
			messages["*"] = (obj, msg) => ClassFunctions.function<Set, Set>(obj, msg, (s1, s2) => s1.Intersection(s2));
			messages["/"] = (obj, msg) => ClassFunctions.function<Set, Set>(obj, msg, (s1, s2) => s1.XOr(s2));
			messages["[]"] = (obj, msg) => ClassFunctions.function<Set, Int>(obj, msg, (s, i) => s[i.Value]);
			messages["length".get()] = (obj, msg) => ClassFunctions.function<Set>(obj, s => s.Length);
			messages["extend"] = (obj, msg) => ClassFunctions.function<Set, IObject>(obj, msg, (s, o) => s.Extend(o));
			messages["clear"] = (obj, msg) => ClassFunctions.function<Set>(obj, s => s.Clear());
			messages["classify"] = (obj, msg) => ClassFunctions.function<Set, Lambda>(obj, msg, (s, l) => s.Classify(l));
		}

		public IObject Revert(IEnumerable<IObject> list) => new Set(list.ToArray());

		public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
	}
}