using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class AssignLastSuccess : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      if (machine.LastSuccess is (true, var (fieldName, success)))
      {
         machine.LastSuccess = nil;
         var _field = machine.CurrentFrame.Fields.New(fieldName, success.Value);
         if (_field)
         {
            return nil;
         }
         else
         {
            return _field.Exception;
         }
      }
      else
      {
         return nil;
      }
   }

   public override string ToString() => "assign.last.success";
}