using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class PushNil : Operation
{
   public override Optional<IObject> Execute(Machine machine) => KNil.NilValue.Just();

   public override string ToString() => "push.nil";
}