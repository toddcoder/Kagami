using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes
{
   public class OptionalClass : BaseClass
   {
      public override string Name => "Optional";

      public override void RegisterMessages()
      {
         base.RegisterMessages();

         messages["value".get()] = (obj, _) => function<IObject>(obj, s => ((IOptional)s).Value);
         messages["isSome".get()] = (obj, _) => function<IObject>(obj, s => (Boolean)((IOptional)s).IsSome);
         messages["isNone".get()] = (obj, _) => function<IObject>(obj, s => (Boolean)((IOptional)s).IsNone);
         messages["map"] = (obj, msg) => function<IObject, Lambda>(obj, msg, (s, l) => ((IOptional)s).Map(l));
         messages["flatMap"] = (obj, msg) => function<IObject, Lambda, Lambda>(obj, msg, (s, l1, l2) => ((IOptional)s).FlatMap(l1, l2));
      }

      public override bool MatchCompatible(BaseClass otherClass) => otherClass.Name is "Some" or "None";

      public override bool AssignCompatible(BaseClass otherClass) => MatchCompatible(otherClass);
   }
}