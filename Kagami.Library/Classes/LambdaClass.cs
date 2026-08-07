using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Kagami.Library.Packages;
using static Kagami.Library.Classes.ClassFunctions;

namespace Kagami.Library.Classes;

public class LambdaClass : BaseClass
{
   public override string Name => "Lambda";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      messages["invoke()"] = (obj, msg) => function<Lambda>(obj, l => invoke(l, msg.Arguments));
      messages[">>(_)"] = (obj, msg) => function<Lambda, Lambda>(obj, msg, (l1, l2) => l1.Join(l2));
      messages["<<(_)"] = (obj, msg) => function<Lambda, Lambda>(obj, msg, (l1, l2) => l2.Join(l1));
      messages["parameterCount".get()] = (obj, _) => function<Lambda>(obj, l => l.ParameterCount);
      messages["fields".get()] = (obj, _) => function<Lambda>(obj, l => l.FieldsInTuple);
      messages["parameters".get()] = (obj, _) => function<Lambda>(obj, l => l.GetParameters());
   }

   public override IObject DefaultValue => new Sys().Identity;

   protected static IObject invoke(Lambda lambda, Arguments arguments)
   {
      return Machine.Current.Invoke(lambda.Invokable, arguments, lambda.Fields, true).Force();
   }
}