using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class IntDivide : TwoOperandOperation
{
   public override string ToString() => "int.divide";

   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      try
      {
         switch (x)
         {
            case Long l1 when y is Long l2:
               return new Long(l1.AsBigInteger() / l2.AsBigInteger());
            case INumeric n1 when y is INumeric n2 && n1.IsPrimitive && n2.IsPrimitive:
            {
               var dx = n1.AsInt32();
               var dy = n2.AsInt32();

               return Int.IntObject(dx / dy).Just();
            }
            default:
               return sendMessage(x, "div(_)", y).Just();
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}