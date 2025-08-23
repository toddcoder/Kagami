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
      if (machine.Peek() is (true, KIndex index))
      {
         machine.Pop();
         return index.EndSucc();
      }

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

               return fieldValue.Just();
            }
            else
            {
               return fail($"{lastFieldName} couldn't be incremented");
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

   public override string ToString() => "post.increment";
}