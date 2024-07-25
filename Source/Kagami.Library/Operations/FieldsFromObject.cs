using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class FieldsFromObject : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         if (value is UserObject userObject)
         {
            var fields = userObject.Fields;
            machine.CurrentFrame.Fields.CopyFrom(fields, (n, _) => n != "self");

            return notMatched<IObject>();
         }
         else
         {
            return failedMatch<IObject>(classNotFound("UserObject"));
         }
      }

      public override string ToString() => "fields.from.object";
   }
}