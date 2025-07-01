using Kagami.Library.Objects;

namespace Kagami.Library.Classes;

public interface ICollectionClass : IEquivalentClass
{
   IObject Revert(IEnumerable<IObject> list);
}