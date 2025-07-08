using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class RunTimeArguments : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      if (x is IRuntimeArguments runtimeArguments)
      {
         if (y is Arguments arguments)
         {
            runtimeArguments.SetArguments(arguments.Value);
            return x.Just();
         }
         else
         {
            return incompatibleClasses(y, "Arguments");
         }
      }
      else
      {
         return incompatibleClasses(x, "UserObjectPlaceholder");
      }
   }

   public override string ToString() => "run.time.arguments";
}