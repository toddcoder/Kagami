using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class OptionalClass : BaseClass, IEquivalentClass
{
   public static TypeConstraint OptionalTypeConstraint => [with([new OptionalClass(), new SomeClass(), new NilClass()])];

   public override string Name => "Optional";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages["value".get()] = (obj, _) => function<IObject>(obj, s => ((IOptional)s).Value);
      messages["isSome".get()] = (obj, _) => function<IObject>(obj, s => (KBoolean)((IOptional)s).IsSome);
      messages["isNone".get()] = (obj, _) => function<IObject>(obj, s => (KBoolean)((IOptional)s).IsNil);
      messages["map(_)"] = (obj, msg) => function<IObject, Lambda>(obj, msg, (s, l) => ((IOptional)s).Map(l));
      messages["flatMap(_,_)"] = (obj, msg) => function<IObject, Lambda, Lambda>(obj, msg, (s, l1, l2) => ((IOptional)s).FlatMap(l1, l2));
      messages["result(_)"] = (obj, msg) => function<IObject, KString>(obj, msg, (s, l) => ((IOptional)s).Result(l));
   }

   public override bool MatchCompatible(BaseClass otherClass) => otherClass.Name is "Some" or "Nil";

   public override bool AssignCompatible(BaseClass otherClass) => MatchCompatible(otherClass);

   public override IObject DefaultValue => KNil.NilValue;

   public TypeConstraint EquivalentTypeConstraint() => OptionalTypeConstraint;
}