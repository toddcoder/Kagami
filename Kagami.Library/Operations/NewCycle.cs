using System.Linq;
using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations
{
   public class NewCycle : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => value switch
      {
         Container list => Cycle.CreateObject(list.List.ToArray()).Matched(),
         ICollection { ExpandForArray: true } collection => Cycle.CreateObject(collection.GetIterator(false).List().ToArray()).Matched(),
         IIterator iterator => Cycle.CreateObject(iterator.List().ToArray()).Matched(),
         _ => new Cycle(value).Matched<IObject>()
      };

      public override string ToString() => "new.cycle";
   }
}