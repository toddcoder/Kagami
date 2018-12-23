using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations
{
   public class FloatDivide : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         if (x is INumeric n1 && y is INumeric n2 && n1.IsPrimitive && n2.IsPrimitive)
         {
            var dx = n1.AsDouble();
            var dy = n2.AsDouble();

            return Float.FloatObject(dx / dy).Matched();
         }
         else
            return sendMessage(x, "/", y).Matched();
      }

      public override string ToString() => "float.divide";
   }
}