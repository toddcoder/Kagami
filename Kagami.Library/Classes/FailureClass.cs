using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
	public class FailureClass : BaseClass
	{
		public override string Name => "Failure";

		public override void RegisterMessages()
		{
			base.RegisterMessages();

			monadMessage();

         messages["error".get()] = (obj, msg) => function<Failure>(obj, s => s.Error);
			messages["isSuccess".get()] = (obj, msg) => function<Success>(obj, s => (Boolean)s.IsSuccess);
			messages["isFailure".get()] = (obj, msg) => function<Success>(obj, s => (Boolean)s.IsFailure);
			messages["map(_<Lambda>)"] = (obj, msg) => function<Success, Lambda>(obj, msg, (s, l) => s.Map(l));
			messages["flatMap(_<Lambda>,_<Lambda>)"] = (obj, msg) => function<Success, Lambda, Lambda>(obj, msg, (s, l1, l2) => s.FlatMap(l1, l2));
			messages["defaultTo(_)"] = (obj, msg) => function<Success, IObject>(obj, msg, (s, o) => o);
		}

		public override bool AssignCompatible(BaseClass otherClass) => otherClass is SuccessClass || otherClass is FailureClass;
   }
}