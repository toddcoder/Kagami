using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class GreaterThan : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y) => x switch
   {
      IObjectCompare xCompare when y is IObjectCompare => KBoolean.BooleanObject(xCompare.Compare(y) > 0).Just(),
      IObjectCompare => greaterThan(x, y),
      Junction junction when y is Junction otherJunction => KBoolean.BooleanObject(junction.Apply(otherJunction, (x, y) => KBoolean.BooleanObject(compareObjects(x, y) > 0)).IsTrue).Just(),
      Junction junction => KBoolean.BooleanObject(junction.Apply(i => KBoolean.BooleanObject(compareObjects(i, y) > 0)).IsTrue).Just(),
      _ => greaterThan(x, y)
   };

   public override string ToString() => "greater.than";
}