using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public class StreamIteratorClass : BaseClass, ICollectionClass
{
   public override string Name => "StreamIterator";

   public IObject Revert(IEnumerable<IObject> list) => new KArray(list);

   public override void RegisterMessages()
   {
      base.RegisterMessages();

      iteratorMessages();
   }

   public override IObject DefaultValue => new StreamIterator(new Iterator(KArray.Empty));

   public BaseClass Equivalent() => new CollectionClass();

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");
}