using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class PreIncrement : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      if (machine is { LastField: (true, var field), LastFieldName: (true, var lastFieldName) })
      {
         if (field.Mutable)
         {
            machine.Pop();
            var fieldValue = field.Value;
            if (fieldValue is IIncrementDecrement incrementDecrement)
            {
               var incrementedValue = incrementDecrement.Increment();
               field.Value = incrementedValue;

               return incrementedValue.Just();
            }
            else
            {
               return sendMessage(fieldValue, "preInc()").Just();
            }
         }
         else
         {
            return immutableField(lastFieldName);
         }
      }
      else
      {
         return fail("No field available");
      }
   }

   public override string ToString() => "pre.increment";
}