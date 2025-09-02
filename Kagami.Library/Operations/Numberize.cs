using Core.Matching;
using Core.Monads;
using Core.Objects;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class Numberize : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      switch (value)
      {
         case KString kString:
         {
            if (kString.Value.IsDate())
            {
               return Date.DateObject(kString.Value.Value().DateTime()).Just();
            }
            else if (kString.Value.IsMatch("['.e']"))
            {
               return Float.FloatObject(kString.Value.Value().Double()).Just();
            }
            else
            {
               return Int.IntObject(kString.Value.Value().Int32()).Just();
            }
         }
         case KChar kChar:
            return Int.IntObject(kChar.Value).Just();
         case INumeric:
            return value.Just();
         default:
            return incompatibleClasses(value, "String");
      }
   }

   public override string ToString() => "numberize";
}