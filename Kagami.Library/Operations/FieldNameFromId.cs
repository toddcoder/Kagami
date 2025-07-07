using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class FieldNameFromId : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      var id = value.Id;
      var result = Module.Global.Value.RetrievedFields.Maybe[id].Map(f => Objects.Some.Object((KString)f)) | None.NoneValue;

      return result.Just();
   }

   public override string ToString() => "field.name.from.id";
}