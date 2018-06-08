using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class RuntimeFunctionClass : BaseClass
   {
      public override string Name => "RuntimeFunction";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["invoke"] = (obj, msg) => function<RuntimeFunction>(obj, rf => rf.Invoke(msg.Arguments.Value));
         messages[">>"] = (obj, msg) => function<RuntimeFunction, Lambda>(obj, msg, (rf, l) => rf.Join(l));
      }
   }
}