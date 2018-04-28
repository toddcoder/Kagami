using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations
{
   public class Raise : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y)
      {
         if (x is INumeric n1 && y is INumeric n2)
         {
            if (n1.IsInt || n2.IsByte)
            {
               var count = n2.AsInt32();
               if (n1.IsInt || n2.IsByte)
               {
                  var accum = 1;
                  var amount = n1.AsInt32();
                  for (var i = 0; i < count; i++)
                     accum *= amount;
                  return Int.Object(accum).Matched();
               }
               else
               {
                  var accum = 1.0;
                  var amount = n1.AsDouble();
                  for (var i = 0; i < count; i++)
                     accum *= amount;
                  return Float.Object(accum).Matched();
               }
            }

            var dx = n1.AsDouble();
            var dy = n2.AsDouble();

            return Float.Object(Math.Pow(dx, dy)).Matched();
         }
         else
            return sendMessage(x, "^", y).Matched();
      }

      public override string ToString() => "raise";
   }
}