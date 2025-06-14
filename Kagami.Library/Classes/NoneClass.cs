using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class NoneClass : BaseClass
{
   public override string Name => "None";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      monadMessages();

      messages["isSome".get()] = (obj, _) => function<None>(obj, n => (KBoolean)n.IsSome);
      messages["isNone".get()] = (obj, _) => function<None>(obj, n => (KBoolean)n.IsNone);
      messages["map(_<Lambda>)"] = (obj, msg) => function<None, Lambda>(obj, msg, (n, l) => n.Map(l));
      messages["flatMap(_<Lambda>,_<Lambda>)"] = (obj, msg) => function<None, Lambda, Lambda>(obj, msg, (n, l1, l2) => n.FlatMap(l1, l2));
      messages["defaultTo(_)"] = (obj, msg) => function<None, IObject>(obj, msg, (_, o) => o);
      messages["canBind".get()] = (obj, _) => function<None>(obj, n => n.CanBind);
      messages["value".get()] = (obj, _) => function<None>(obj, n => n.Value);
   }

   public override bool AssignCompatible(BaseClass otherClass) => otherClass is SomeClass or NoneClass;
}