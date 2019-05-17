using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
	public class CycleClass : BaseClass
	{
		public override string Name => "Cycle";

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			collectionMessages();

			messages["items".get()] = (obj, msg) => function<Cycle>(obj, c => c.Items);
		}
	}
}