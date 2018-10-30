using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
	public class ResultClass : BaseClass
	{
		public override string Name => "Result";

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			messages["value".get()] = (obj, msg) => function<IObject>(obj, s => ((IResult)s).Value);
			messages["error".get()] = (obj, msg) => function<IObject>(obj, s => ((IResult)s).Error);
			messages["isSuccess".get()] = (obj, msg) => function<IObject>(obj, s => (Boolean)((IResult)s).IsSuccess);
			messages["isFailure".get()] = (obj, msg) => function<IObject>(obj, s => (Boolean)((IResult)s).IsFailure);
			messages["map"] = (obj, msg) => function<IObject, Lambda>(obj, msg, (s, l) => ((IResult)s).Map(l));
			messages["flatMap"] = (obj, msg) => function<IObject, Lambda, Lambda>(obj, msg, (s, l1, l2) => ((IResult)s).FlatMap(l1, l2));
      }

		public override bool AssignCompatible(BaseClass otherClass) => otherClass.Name == "Success" || otherClass.Name == "Failure";
	}
}