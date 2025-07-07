using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class AssignLastSome : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      if (machine.LastSome is (true, var (fieldName, some)))
      {
         machine.LastSome = nil;
         var _field = machine.CurrentFrame.Fields.New(fieldName, some.Value);
         if (_field)
         {
            return KBoolean.True.Just();
         }
         else
         {
            return _field.Exception;
         }
      }
      else
      {
         return KBoolean.False.Just();
      }
   }

   public override string ToString() => "assign.last.some";
}