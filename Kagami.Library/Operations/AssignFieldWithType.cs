using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class AssignFieldWithType(string fieldName, string className) : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      var _field = machine.Find(fieldName, true);
      if (_field is (true, var field))
      {
         var _class = Module.Global.Value.Class(className);
         if (_class is (true, var cls))
         {
            var typeConstraint = new TypeConstraint([cls]);
            field.TypeConstraint = typeConstraint;
            field.Value = value;

            return nil;
         }
         else
         {
            return classNotFound(className);
         }
      }
      else
      {
         return fieldNotFound(fieldName);
      }
   }

   public override string ToString() => "assign.field.with.type";
}