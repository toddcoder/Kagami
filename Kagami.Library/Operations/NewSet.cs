using System.Linq;
using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewSet : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      return value switch
      {
         Sequence list => new Set(list.List.ToArray()),
         ICollection collection and not KString => new Set(collection.GetIterator(false).List().ToArray()),
         IIterator iterator => new Set(iterator.List().ToArray()),
         _ => new Set(value)
      };
   }

   public override string ToString() => "new.set";
}