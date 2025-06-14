using Kagami.Library.Objects;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class FailureClass : BaseClass
{
   public override string Name => "Failure";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      monadMessages();

      messages["error".get()] = (obj, _) => function<Failure>(obj, f => f.Error);
      messages["isSuccess".get()] = (obj, _) => function<Failure>(obj, f => (KBoolean)f.IsSuccess);
      messages["isFailure".get()] = (obj, _) => function<Failure>(obj, f => (KBoolean)f.IsFailure);
      messages["map(_<Lambda>)"] = (obj, msg) => function<Failure, Lambda>(obj, msg, (f, l) => f.Map(l));
      messages["flatMap(_<Lambda>,_<Lambda>)"] = (obj, msg) => function<Failure, Lambda, Lambda>(obj, msg, (f, l1, l2) => f.FlatMap(l1, l2));
      messages["defaultTo(_)"] = (obj, msg) => function<Failure, IObject>(obj, msg, (_, o) => o);
      messages["canBind".get()] = (obj, _) => function<Failure>(obj, f => f.CanBind);
      messages["value".get()] = (obj, _) => function<Failure>(obj, f => f.Value);
   }

   public override bool AssignCompatible(BaseClass otherClass) => otherClass is SuccessClass or FailureClass;
}