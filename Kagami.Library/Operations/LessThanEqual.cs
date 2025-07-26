using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

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
         default:
            return fail($"{x.Image} must be comparable");
      }
   }

   public override string ToString() => "less.than.equal";
}