using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class PostDecrement : Operation
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
               var decrementedValue = incrementDecrement.Decrement();
               field.Value = decrementedValue;

               return fieldValue.Just();
            }
            else
            {
               return fail($"{lastFieldName} couldn't be decremented");
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

   public override string ToString() => "post.decrement";
}