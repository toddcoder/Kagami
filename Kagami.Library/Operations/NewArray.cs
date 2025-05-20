using System.Linq;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class NewArray : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Container list => KArray.CreateObject(list.List.ToArray()).Just(),
      IKeyValue => KArray.CreateObject([value]).Just(),
      ICollection { ExpandForArray: true } collection => KArray.CreateObject(collection.GetIterator(false).List().ToArray()).Just(),
      IIterator iterator => KArray.CreateObject(iterator.List().ToArray()).Just(),
      _ => new KArray(value)
   };

   public override string ToString() => "new.array";
}