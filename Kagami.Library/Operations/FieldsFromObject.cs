using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class FieldsFromObject : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         if (value is UserObject userObject)
         {
            var fields = userObject.Fields;
            machine.CurrentFrame.Fields.CopyFrom(fields);
            return notMatched<IObject>();
         }
         else
            return failedMatch<IObject>(classNotFound("UserObject"));
      }

      public override string ToString() => "fields.from.object";
   }
}