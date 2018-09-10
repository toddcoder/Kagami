using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
	public class SkipTakeClass : BaseClass
	{
		public override string Name => "SkipTake";

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			registerMessage("skip".Selector("<Int>"), (obj, msg) => function<SkipTake, Int>(obj, msg, (st, i) => st.Skip(i.Value)));
			registerMessage("take".Selector("<Int>"), (obj, msg) => function<SkipTake, Int>(obj, msg, (st, i) => st.Take(i.Value)));
			registerMessage("literal".Selector(1), (obj, msg) => function<SkipTake, IObject>(obj, msg, (st, o) => st.Literal(o)));
			registerMessage("takeRest".Selector(0), (obj, msg) => function<SkipTake>(obj, st => st.TakeRest()));
			registerMessage("fullResult".get(), (obj, msg) => function<SkipTake>(obj, st => st.FullResult));
		}
	}
}