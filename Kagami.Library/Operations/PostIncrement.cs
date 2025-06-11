using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class PostIncrement : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      if (machine.LastField is (true, var field))
      {
         if (field.Mutable)
         {
            var fieldValue = field.Value;
            if (fieldValue is IIncrementDecrement incrementDecrement)
            {
               var incrementedValue = incrementDecrement.Increment();
               field.Value = incrementedValue;

               return fieldValue.Just();
            }
            else
            {
               return fail("Value couldn't be incremented");
            }
         }
         else
         {
            return immutableField("field");
         }
      }
      else
      {
         return fail("No field available");
      }
   }

   public override string ToString() => "post.increment";
}