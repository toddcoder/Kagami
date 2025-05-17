using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class BNot : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Int i)
      {
         return Int.IntObject(~i.Value).Just();
      }
      else
      {
         return incompatibleClasses(value, "Int");
      }
   }

   public override string ToString() => "bnot";
}