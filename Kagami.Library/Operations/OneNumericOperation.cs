using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public abstract class OneNumericOperation : OneOperandOperation
{
   public abstract Optional<IObject> Execute(Machine machine, INumeric x);

   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is INumeric numeric)
      {
         return Execute(machine, numeric);
      }
      else
      {
         return incompatibleClasses(value, "Numeric");
      }
   }
}