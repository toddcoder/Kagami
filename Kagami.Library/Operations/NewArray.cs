using System.Linq;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class NewArray : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Container list => Array.CreateObject(list.List.ToArray()).Just(),
      IKeyValue => Array.CreateObject([value]).Just(),
      ICollection { ExpandForArray: true } collection => Array.CreateObject(collection.GetIterator(false).List().ToArray()).Just(),
      IIterator iterator => Array.CreateObject(iterator.List().ToArray()).Just(),
      _ => new Array(value)
   };

   public override string ToString() => "new.array";
}