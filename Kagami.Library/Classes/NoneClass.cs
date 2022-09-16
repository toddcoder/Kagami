using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class NoneClass : BaseClass
   {
      public override string Name => "None";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         monadMessage();

         messages["isSome".get()] = (obj, _) => function<None>(obj, n => (Boolean)n.IsSome);
         messages["isNone".get()] = (obj, _) => function<None>(obj, n => (Boolean)n.IsNone);
         messages["map(_<Lambda>)"] = (obj, msg) => function<None, Lambda>(obj, msg, (n, l) => n.Map(l));
         messages["flatMap(_<Lambda>,_<Lambda>)"] = (obj, msg) => function<None, Lambda, Lambda>(obj, msg, (n, l1, l2) => n.FlatMap(l1, l2));
         messages["defaultTo(_)"] = (obj, msg) => function<None, IObject>(obj, msg, (_, o) => o);
         messages["canBind".get()] = (obj, _) => function<None>(obj, n => n.CanBind);
      }

      public override bool AssignCompatible(BaseClass otherClass) => otherClass is SomeClass or NoneClass;
   }
}