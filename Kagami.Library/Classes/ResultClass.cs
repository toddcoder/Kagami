using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class ResultClass : BaseClass, IEquivalentClass
{
   public static TypeConstraint ResultTypeConstraint => [with([new ResultClass(), new SuccessClass(), new FailureClass()])];

   public override string Name => "Result";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages["value".get()] = (obj, _) => function<IObject>(obj, s => ((IResult)s).Value);
      messages["error".get()] = (obj, _) => function<IObject>(obj,
         _ => new ProtocolWrapper(obj, Protocols.Protocols.Get("PError").Required(messageProtocolNotFound("PError"))));
      messages["isSuccess".get()] = (obj, _) => function<IObject>(obj, s => (KBoolean)((IResult)s).IsSuccess);
      messages["isFailure".get()] = (obj, _) => function<IObject>(obj, s => (KBoolean)((IResult)s).IsFailure);
      messages["map(_<Lambda>)"] = (obj, msg) => function<IObject, Lambda>(obj, msg, (s, l) => ((IResult)s).Map(l));
      messages["flatMap(_<Lambda>,_<Lambda>)"] =
         (obj, msg) => function<IObject, Lambda, Lambda>(obj, msg, (s, l1, l2) => ((IResult)s).FlatMap(l1, l2));
      messages["optional()"] = (obj, _) => function<IObject>(obj, s => ((IResult)s).Optional());
   }

   public override bool AssignCompatible(BaseClass otherClass) => otherClass.Name is "Success" or "Failure";

   public override IObject DefaultValue => new Failure("No value");

   public TypeConstraint EquivalentTypeConstraint() => ResultTypeConstraint;
}