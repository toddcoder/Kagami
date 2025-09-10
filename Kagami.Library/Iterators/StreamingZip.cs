using Kagami.Library.Objects;

namespace Kagami.Library.Iterators;

public class StreamingZip(ICollection collection) : StreamingZipIterator(collection.GetIterator(false))
{
   public override string ToString() => $"zip({((IObject)collection).ClassName})";
}