using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class DefineNewField(bool mutable, string fieldName, string className) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      if (Module.Global.Value.Class(className) is (true, var baseClass))
      {
         try
         {
            var defaultValue = baseClass.DefaultValue;
            var _field = machine.CurrentFrame.Fields.New(fieldName, FieldType.Assignment, defaultValue, mutable);
            if (_field)
            {
               return defaultValue.Just();
            }
            else
            {
               return _field.Exception;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }
      else
      {
         return classNotFound(className);
      }
   }

   public override string ToString() => "define.new.field";
}