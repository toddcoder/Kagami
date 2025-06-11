using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class GetField : Operation
{
   protected string fieldName;

   public GetField(string fieldName) => this.fieldName = fieldName;

   public override Optional<IObject> Execute(Machine machine)
   {
      var _field = machine.Find(fieldName, true);
      if (_field is (true, var field))
      {
         machine.LastField = field;
         return field.Value.Just();
      }
      else if (_field.Exception is (true, var exception))
      {
         machine.LastField = nil;
         return exception;
      }
      else
      {
         machine.LastField = nil;
         return fieldNotFound(fieldName);
      }
   }

   public override string ToString() => $"get.field({fieldName})";
}