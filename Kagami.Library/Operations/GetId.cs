using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class GetId : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => KString.StringObject(value.Id.ToString()).Just();

   public override string ToString() => "get.id";
}