using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
	public class SuccessClass : BaseClass
	{
		public override string Name => "Success";

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			monadMessage();

         messages["value".get()] = (obj, msg) => function<Success>(obj, s => s.Value);
			messages["isSuccess".get()] = (obj, msg) => function<Success>(obj, s => (Boolean)s.IsSuccess);
			messages["isFailure".get()] = (obj, msg) => function<Success>(obj, s => (Boolean)s.IsFailure);
			messages["map"] = (obj, msg) => function<Success, Lambda>(obj, msg, (s, l) => s.Map(l));
			messages["flatMap"] = (obj, msg) => function<Success, Lambda, Lambda>(obj, msg, (s, l1, l2) => s.FlatMap(l1, l2));
			messages["defaultTo"] = (obj, msg) => function<Success, IObject>(obj, msg, (s, o) => s.Value);
      }

		public override bool AssignCompatible(BaseClass otherClass) => otherClass is SuccessClass || otherClass is FailureClass;
   }
}