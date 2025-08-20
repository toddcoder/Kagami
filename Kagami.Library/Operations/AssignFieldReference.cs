using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class AssignFieldReference(string sourceFieldName, string targetFieldName) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      try
      {
         var _sourceField = machine.Find(sourceFieldName, true);
         if (_sourceField is (true, var sourceField))
         {
            machine.CurrentFrame.Fields.NewRefField(sourceField, targetFieldName, FieldType.Assignment, sourceField.TypeConstraint, true, true);
            return sourceField.Value.Just();
         }
         else
         {
            return fieldNotFound(sourceFieldName);
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override string ToString() => "assign.field.reference";
}