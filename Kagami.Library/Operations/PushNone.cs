using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class PushNone : Operation
{
   public override Optional<IObject> Execute(Machine machine) => None.NoneValue.Just();

   public override string ToString() => "push.none";
}