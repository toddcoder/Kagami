using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class LambaClass : BaseClass
   {
      public override string Name => "Lambda";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["invoke"] = (obj, msg) => function<Lambda>(obj, l => invoke(l, msg.Arguments));
      }

      protected static IObject invoke(Lambda lambda, Arguments arguments)
      {
         return Machine.Current.Invoke(lambda.Invokable, arguments, lambda.Fields).Value;
      }
   }
}