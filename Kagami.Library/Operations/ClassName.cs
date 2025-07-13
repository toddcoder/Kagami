using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class ClassName : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => KString.StringObject(value.ClassName).Just();

   public override string ToString() => "class.name";
}