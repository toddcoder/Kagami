using Kagami.Library;
using Kagami.Library.Classes;
using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Awk
{
	public class AwkRecordClass : BaseClass
	{
		public override string Name => "AwkRecord";

		public override void RegisterMessages()
		{
			base.RegisterMessages();
			textFindingMessages();

			registerMessage("[]", (obj, msg) => function<AwkRecord, Int>(obj, msg, (ar, i) => ar[i.Value]));
			registerMessage("[]=".Selector("<Int>", "<String>"),
				(obj, msg) => function<AwkRecord, Int, KString>(obj, msg, (ar, i, s) => ar[i.Value] = s.Value));
		}
	}
}