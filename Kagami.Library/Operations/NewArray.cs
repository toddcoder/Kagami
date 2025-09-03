using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class NewArray : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Sequence list => KArray.CreateObject(list.List.ToArray()).Just(),
      IKeyValue { Key: INumeric nKey, Value: INumeric nValue } => new Objects.SkipTake(nKey.AsInt32(), nValue.AsInt32()),
      IKeyValue => KArray.CreateObject([value]).Just(),
      ICollection { ExpandForArray: true } collection => KArray.CreateObject(collection.GetIterator(false).List().ToArray()).Just(),
      IIterator iterator => KArray.CreateObject(iterator.List().ToArray()).Just(),
      Junction junction => KArray.CreateObject(junction.Items).Just(),
      _ => new KArray(value)
   };

   public override string ToString() => "new.array";
}