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

         messages["value".get()] = (obj, msg) => function<Some>(obj, s => s.Value);
         messages["isSome".get()] = (obj, msg) => function<Some>(obj, s => (Boolean)s.IsSome);
         messages["isNil".get()] = (obj, msg) => function<Some>(obj, s => (Boolean)s.IsNil);
         messages["map"] = (obj, msg) => function<Some, Lambda>(obj, msg, (s, l) => s.Map(l));
         messages["flatMap"] = (obj, msg) => function<Some, Lambda, Lambda>(obj, msg, (s, l1, l2) => s.FlatMap(l1, l2));
      }

      public override bool AssignCompatible(BaseClass otherClass) => otherClass is SomeClass || otherClass is NilClass;
   }
}