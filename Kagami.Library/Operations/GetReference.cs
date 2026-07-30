using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class GetReference(string fieldName) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var _field = machine.Find(fieldName, true);
      if (_field is (true, var field))
      {
         return new Reference(field);
      }
      else if (_field.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return AllExceptions.fieldNotFound(fieldName);
      }
   }

   public override string ToString() => $"get.reference({fieldName})";
}