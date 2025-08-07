using Core.Monads;
using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingState(LazyIterator iterator)
{
   protected Queue<IObject> queue = new();

   public Maybe<IObject> Next() => iterator.Next();
}