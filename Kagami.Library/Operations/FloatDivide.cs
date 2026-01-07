using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class FloatDivide : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      try
      {
         switch (x)
         {
            case Int i1 when y is Int i2:
               return Int.IntObject(i1.Value / i2.Value).Just();
            case Long l1 when y is Long l2:
               return Long.LongObject(l1.Value / l2.Value).Just();
            case INumeric n1 when y is INumeric n2 && n1.IsPrimitive && n2.IsPrimitive:
            {
               var dx = n1.AsDouble();
               var dy = n2.AsDouble();

               return Float.FloatObject(dx / dy).Just();
            }
            case KIndex index when y is Int i:
               return index.Contract(i.Value);
            default:
               return sendMessage(x, "/(_)", y).Just();
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override string ToString() => "float.divide";
}