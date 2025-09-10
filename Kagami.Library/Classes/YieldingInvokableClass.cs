using Kagami.Library.Objects;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Classes;

public class YieldingInvokableClass : BaseClass, IEquivalentClass
{
   public override string Name => "YieldingInvokable";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      collectionMessages();
   }

   public override IObject DefaultValue => throw noDefaultValue("YieldingInvokable");

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Iterator");
}