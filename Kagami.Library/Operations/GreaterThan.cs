using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class GreaterThan : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      if (x is IObjectCompare xCompare)
      {
         if (y is IObjectCompare)
         {
            return KBoolean.BooleanObject(xCompare.Compare(y) > 0).Just();
         }
         else
         {
            return fail($"{y.Image} must be comparable");
         }
      }
      else
      {
         return fail($"{x.Image} must be comparable");
      }
   }

   public override string ToString() => "greater.than";
}