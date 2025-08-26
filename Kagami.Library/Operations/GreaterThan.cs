using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.Objects.ObjectFunctions;

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
            return greaterThan(x, y);
         }
      }
      else
      {
         return greaterThan(x, y);
      }
   }

   public override string ToString() => "greater.than";
}