using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class UniqueString : Operation
{
   public override Optional<IObject> Execute(Machine machine) => KString.StringObject(Guid.NewGuid().ToString()).Just();

   public override string ToString() => "unique.string";
}