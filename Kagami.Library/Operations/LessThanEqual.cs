using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class LessThanEqual : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      switch (x)
      {
         case Before before:
            return KBoolean.BooleanObject(before.Compare(y) <= 0).Just();
         case KBoolean { Value: false }:
            return KBoolean.False.Just();
         case IObjectCompare xCompare when y is IObjectCompare:
         {
            if (xCompare.Compare(y) <= 0)
            {
               return new Before(y);
            }
            else
            {
               return KBoolean.False.Just();
            }
         }
         case IObjectCompare:
            return fail($"{y.Image} must be comparable");
         case Junction junction:
         {
            if (y is Junction otherJunction)
            {
               return KBoolean.BooleanObject(junction.Apply(otherJunction, (x, y) => KBoolean.BooleanObject(compareObjects(x, y) <= 0)).IsTrue).Just();
            }
            else
            {
               return KBoolean.BooleanObject(junction.Apply(i => KBoolean.BooleanObject(compareObjects(i, y) <= 0)).IsTrue).Just();
            }
         }
         default:
            return lessThanEqual(x, y);
      }
   }

   public override string ToString() => "less.than.equal";
}