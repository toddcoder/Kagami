using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class LambdaCapture : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      if (value is Lambda lambda)
      {
         lambda.Capture(machine);
      }
      else
      {
         return incompatibleClasses(value, "Lambda");
      }

      return value.Just();
   }

   public override string ToString() => "lambda.capture";
}