using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class IteratorClass : BaseClass, IEquivalentClass
{
   public override string Name => "Iterator";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      iteratorMessages();
   }

   public override IObject DefaultValue => (IObject)KArray.Empty.GetIterator(false);

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Iterator");
}