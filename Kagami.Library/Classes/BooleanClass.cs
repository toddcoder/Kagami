using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class BooleanClass : BaseClass
{
   public override string Name => "Boolean";

   public override void RegisterClassMessages()
   {
      base.RegisterClassMessages();

      classMessages["parse(_)"] = (_, msg) => parse(msg.Arguments[0].AsString);
   }

   public override IObject DefaultValue => KBoolean.False;

   protected static IObject parse(string source) => source switch
   {
      "false" => Success.Object(KBoolean.False, TypeConstraint.SingleType("Boolean")),
      "true" => Success.Object(KBoolean.True, TypeConstraint.SingleType("Boolean")),
      _ => Failure.Object($"Couldn't understand {source}")
   };
}