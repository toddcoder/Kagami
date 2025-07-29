using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class NilClass : BaseClass
{
   public override string Name => "Nil";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      monadMessages();

      messages["isSome".get()] = (obj, _) => function<KNil>(obj, n => (KBoolean)n.IsSome);
      messages["isNil".get()] = (obj, _) => function<KNil>(obj, n => (KBoolean)n.IsNil);
      messages["map(_<Lambda>)"] = (obj, msg) => function<KNil, Lambda>(obj, msg, (n, l) => n.Map(l));
      messages["flatMap(_<Lambda>,_<Lambda>)"] = (obj, msg) => function<KNil, Lambda, Lambda>(obj, msg, (n, l1, l2) => n.FlatMap(l1, l2));
      messages["defaultTo(_)"] = (obj, msg) => function<KNil, IObject>(obj, msg, (_, o) => o);
      messages["canBind".get()] = (obj, _) => function<KNil>(obj, n => n.CanBind);
      messages["value".get()] = (obj, _) => function<KNil>(obj, n => n.Value);
      messages["result(_)"] = (obj, msg) => function<IObject, KString>(obj, msg, (s, l) => ((IOptional)s).Result(l));
   }

   public override bool AssignCompatible(BaseClass otherClass) => otherClass is SomeClass or NilClass;

   public override IObject DefaultValue => KNil.NilValue;
}