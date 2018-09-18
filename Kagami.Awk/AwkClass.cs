using Kagami.Library;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Awk
{
	public class AwkClass : PackageClass
	{
		public override string Name => "Awk";

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			registerPackageFunction("awkify".Selector("<String>"),
				(obj, msg) => function<Awk, String>(obj, msg, (a, s) => a.Awkify(s.Value)));
		}
	}
}