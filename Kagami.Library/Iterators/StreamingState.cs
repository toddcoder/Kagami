using Kagami.Library.Classes;
using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingState(IObject next, ICollectionClass collectionClass)
{
   protected IObject next = next;

   public IObject Next
   {
      get => next;
      set => next = value;
   }

   public ICollectionClass CollectionClass => collectionClass;
}