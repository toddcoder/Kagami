using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class PushCallStack : Operation
{
   public override Optional<IObject> Execute(Machine machine) => KString.StringObject(machine.CallStack).Just();

   public override string ToString() => "push.call.stack";
}