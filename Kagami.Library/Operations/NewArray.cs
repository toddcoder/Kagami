using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class NewArray(Maybe<TypeConstraint> _typeConstraint) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Sequence list => KArray.CreateObject([.. list.List], _typeConstraint).Just(),
      IKeyValue { Key: INumeric nKey, Value: INumeric nValue } => new Objects.SkipTake(nKey.AsInt32(), nValue.AsInt32()),
      IKeyValue => KArray.CreateObject([value], _typeConstraint).Just(),
      ICollection { ExpandForArray: true } collection => KArray.CreateObject([.. collection.GetIterator(false).List()], _typeConstraint).Just(),
      IIterator iterator => KArray.CreateObject([.. iterator.List()], _typeConstraint).Just(),
      Junction junction => KArray.CreateObject(junction.Items, _typeConstraint).Just(),
      UserObject uo when supportsIteration(uo) => KArray.CreateObject(new ObjectCollection(uo).GetIterator(false).List(), _typeConstraint).Just(),
      _ => new KArray(value) { TypeConstraint = _typeConstraint }
   };

   public override string ToString() => "new.array";
}