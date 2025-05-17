using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class FieldExists : Operation
{
   protected string fieldName;

   public FieldExists(string fieldName) => this.fieldName = fieldName;

   public override Optional<IObject> Execute(Machine machine)
   {
      var _field = machine.Find(fieldName, true);
      if (_field)
      {
         return Boolean.True.Just();
      }
      else if (_field.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return Boolean.False.Just();
      }
   }

   public override string ToString() => $"field.exists({fieldName})";
}