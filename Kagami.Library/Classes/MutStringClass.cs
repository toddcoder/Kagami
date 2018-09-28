using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
	public class MutStringClass : BaseClass
	{
		public override string Name => "MutString";

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			collectionMessages();
			formatMessage<MutString>();
			sliceableMessages();
			compareMessages();
			rangeMessages();
			textFindingMessages();

			registerMessage("<<", (obj, msg) => function<MutString, IObject>(obj, msg, (m, o) => m.Append(o)));
			registerMessage("[](_<Int>)", (obj, msg) => function<MutString, Int>(obj, msg, (m, i) => m[i.Value]));
			registerMessage("[]=(_<Int>,_<Char>)", (obj, msg) => function<MutString, Int, Char>(obj, msg, (m, i, v) => m[i.Value] = v));
		}
	}
}