using Kagami.Library.Classes;

namespace Kagami.Library.Mixins
{
	public class CollectingClass : BaseClass
	{
		public override string Name => "Collecting";

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			collectionMessages();
		}
	}
}