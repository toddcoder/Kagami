using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class EmptyTuple : Operation
{
   public override Optional<IObject> Execute(Machine machine) => KTuple.Empty;

   public override string ToString() => "empty.tuple";
}