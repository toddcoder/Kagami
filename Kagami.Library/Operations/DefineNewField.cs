using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class DefineNewField(bool mutable, string fieldName, TypeConstraint typeConstraint) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      try
      {
         var defaultValue = mutable ? typeConstraint.Comparisands[0].DefaultValue : Unassigned.Value;
         var _field = machine.CurrentFrame.Fields.New(fieldName, FieldType.Assignment, typeConstraint, defaultValue, mutable, true);
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

   public override string ToString() => "define.new.field";
}