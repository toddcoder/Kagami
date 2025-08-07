using Kagami.Library.Classes;
using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingState(IObject next, ICollection collection, ICollectionClass collectionClass)
{
   protected IObject next = next;

   public IObject Next
   {
      get => next;
      set => next = value;
   }

   public ICollection Collection => collection;

   public ICollectionClass CollectionClass => collectionClass;
}