using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class GetField(string fieldName) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var _field = machine.Find(fieldName, true);
      if (_field is (true, var field))
      {
         machine.LastField = field;
         machine.LastFieldName = fieldName;
         Module.Global.Value.RetrievedFields[field.Value.Id] = fieldName;

         var value = field.Value;

         switch (value)
         {
            case Objects.Some some:
               machine.LastSome = (fieldName, some);
               break;
            case Objects.Success success:
               machine.LastSuccess = (fieldName, success);
               break;
         }

         return value.Just();
      }
      else if (_field.Exception is (true, var exception))
      {
         machine.LastField = nil;
         machine.LastFieldName = nil;
         return exception;
      }
      else
      {
         machine.LastField = nil;
         machine.LastFieldName= nil;
         return fieldNotFound(fieldName);
      }
   }

   public override string ToString() => $"get.field({fieldName})";
}