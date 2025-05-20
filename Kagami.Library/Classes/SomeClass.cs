using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class SomeClass : BaseClass
   {
      public override string Name => "Some";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         monadMessage();

         messages["value".get()] = (obj, _) => function<Some>(obj, s => s.Value);
         messages["isSome".get()] = (obj, _) => function<Some>(obj, s => (KBoolean)s.IsSome);
         messages["isNone".get()] = (obj, _) => function<Some>(obj, s => (KBoolean)s.IsNone);
         messages["map(_<Lambda>)"] = (obj, msg) => function<Some, Lambda>(obj, msg, (s, l) => s.Map(l));
         messages["flatMap(_<Lambda>,_<Lambda>)"] = (obj, msg) => function<Some, Lambda, Lambda>(obj, msg, (s, l1, l2) => s.FlatMap(l1, l2));
         messages["defaultTo(_)"] = (obj, msg) => function<Some, IObject>(obj, msg, (s, _) => s.Value);
         messages["canBind".get()] = (obj, _) => function<Some>(obj, s => s.CanBind);
      }

      public override bool AssignCompatible(BaseClass otherClass) => otherClass is SomeClass or NoneClass;
   }
}