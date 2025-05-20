using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class NewArray : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Container list => KArray.CreateObject([.. list.List]).Just(),
      IKeyValue => KArray.CreateObject([value]).Just(),
      ICollection { ExpandForArray: true } collection => KArray.CreateObject([.. collection.GetIterator(false).List()]).Just(),
      IIterator iterator => KArray.CreateObject([.. iterator.List()]).Just(),
      _ => new KArray(value)
   };

   public override string ToString() => "new.array";
}