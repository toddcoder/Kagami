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

         messages["value".get()] = (obj, _) => function<Success>(obj, s => s.Value);
         messages["isSuccess".get()] = (obj, _) => function<Success>(obj, s => (KBoolean)s.IsSuccess);
         messages["isFailure".get()] = (obj, _) => function<Success>(obj, s => (KBoolean)s.IsFailure);
         messages["map(_<Lambda>)"] = (obj, msg) => function<Success, Lambda>(obj, msg, (s, l) => s.Map(l));
         messages["flatMap(_<Lambda>,_<Lambda>)"] = (obj, msg) => function<Success, Lambda, Lambda>(obj, msg, (s, l1, l2) => s.FlatMap(l1, l2));
         messages["defaultTo(_)"] = (obj, msg) => function<Success, IObject>(obj, msg, (s, _) => s.Value);
         messages["canBind".get()] = (obj, _) => function<Success>(obj, s => s.CanBind);
      }

      public override bool AssignCompatible(BaseClass otherClass) => otherClass is SuccessClass or FailureClass;
   }
}