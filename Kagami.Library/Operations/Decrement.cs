using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class Decrement : OneNumericOperation
{
   public override Optional<IObject> Execute(Machine machine, INumeric x)
   {
      if (machine is { LastField: (true, var field), LastFieldName: (true, var lastFieldName) })
      {
         if (field.Mutable)
         {
            machine.Pop();
            var fieldValue = field.Value;
            if (fieldValue is IIncrementDecrement incrementDecrement)
            {
               var incrementedValue = incrementDecrement.Decrement(x.AsInt32());
               field.Value = incrementedValue;

               return incrementedValue.Just();
            }
            else
            {
               return fail($"{lastFieldName} couldn't be incremented");
            }
         }
         else
         {
            return noDefaultValue(lastFieldName);
         }
      }
      else
      {
         return fail("No field available");
      }
   }

   public override string ToString() => "decrement";
}