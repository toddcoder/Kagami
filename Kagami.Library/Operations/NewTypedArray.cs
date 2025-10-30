using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewTypedArray(TypeConstraint typeConstraint) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Sequence list => TypedArray.CreateObject(list.List.ToArray(), typeConstraint).Just(),
      IKeyValue { Key: INumeric nKey, Value: INumeric nValue } => new Objects.SkipTake(nKey.AsInt32(), nValue.AsInt32()),
      IKeyValue => TypedArray.CreateObject([value], typeConstraint).Just(),
      ICollection { ExpandForArray: true } collection => TypedArray.CreateObject([.. collection.GetIterator(false).List()], typeConstraint).Just(),
      IIterator iterator => TypedArray.CreateObject([.. iterator.List()], typeConstraint).Just(),
      Junction junction => TypedArray.CreateObject(junction.Items, typeConstraint).Just(),
      _ => new TypedArray(value, typeConstraint)
   };

   public override string ToString() => $"new.typed.array({typeConstraint.AsString})";
}