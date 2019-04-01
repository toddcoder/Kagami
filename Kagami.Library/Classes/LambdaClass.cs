using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class LambdaClass : BaseClass
   {
      public override string Name => "Lambda";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["invoke"] = (obj, msg) => function<Lambda>(obj, l => invoke(l, msg.Arguments));
         messages[">>"] = (obj, msg) => function<Lambda, Lambda>(obj, msg, (l1, l2) => l1.Join(l2));
      }

      protected static IObject invoke(Lambda lambda, Arguments arguments)
      {
	      return Machine.Current.Invoke(lambda.Invokable, arguments, lambda.Fields)
		      .RequiredCast<IObject>(() => "Return value required");
      }
   }
}