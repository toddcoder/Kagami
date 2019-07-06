using Kagami.Library.Objects;

namespace Kagami.Library.Classes
{
	public class BooleanClass : BaseClass
	{
		public override string Name => "Boolean";

		public override void RegisterClassMessages()
		{
			base.RegisterClassMessages();

			classMessages["parse(_)"] = (bc, msg) => parse(msg.Arguments[0].AsString);
		}

		static IObject parse(string source)
		{
			switch (source)
			{
				case "false":
					return Success.Object(Boolean.False);
				case "true":
					return Success.Object(Boolean.True);
				default:
					return Failure.Object($"Couldn't understand {source}");
			}
		}
	}
}