using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

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
               return sendMessage(fieldValue, "postDec()").Just();
            }
         }
         else
         {
            return immutableField(lastFieldName);
         }
      }
      else
      {
         var _value = machine.Pop();
         if (_value is (true, var value))
         {
            try
            {
               return sendMessage(value, "postDec()").Just();
            }
            catch (Exception exception)
            {
               return exception;
            }
         }
         else
         {
            return _value.Exception;
         }
      }
   }

   public override string ToString() => "post.decrement";
}