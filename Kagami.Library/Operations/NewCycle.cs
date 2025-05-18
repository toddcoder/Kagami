using System.Linq;
using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewCycle : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Container list => Cycle.CreateObject(list.List.ToArray()).Just(),
      ICollection { ExpandForArray: true } collection => Cycle.CreateObject(collection.GetIterator(false).List().ToArray()).Just(),
      IIterator iterator => Cycle.CreateObject(iterator.List().ToArray()).Just(),
      _ => new Cycle(value)
   };

   public override string ToString() => "new.cycle";
}