using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class LazyIteratorClass : BaseClass, IEquivalentClass
{
   public override string Name => "LazyIterator";

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      iteratorMessages();
   }

   public override IObject DefaultValue => new LazyIterator(KArray.Empty);

   public TypeConstraint EquivalentTypeConstraint() => TypeConstraint.FromList("Iterator");
}