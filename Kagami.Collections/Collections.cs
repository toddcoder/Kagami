using System.Linq;
using Kagami.Library.Objects;
using Kagami.Library.Packages;
using Kagami.Library.Runtime;

namespace Kagami.Collections
{
    public class Collections : Package
    {
       public override string ClassName => "Collections";

       public override void LoadTypes(Module module)
       {
          module.RegisterClass(new CollectionsClass());
          module.RegisterClass(new SetClass());
       }

       public Set Set(ICollection collection) => new Set(collection.GetIterator(false).List().ToArray());

       public Set Set(IObject[] objects) => new Set(objects);

       public Set Set(Set otherSet) => new Set(otherSet);
    }
}
