using Kagami.Library.Classes;
using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingIteratorClass : BaseClass, ICollectionClass
{
   public override string Name => "StreamingIterator";

   public override IObject DefaultValue => new StreamingIterator(new Iterator(KArray.Empty));

   public TypeConstraint TypeConstraint() => Objects.TypeConstraint.FromList("Collection");

   public IObject Revert(IEnumerable<IObject> list) => new KArray(list);

   public override void RegisterMessages()
   {
      base.RegisterMessages();
      iteratorMessages();
   }
}