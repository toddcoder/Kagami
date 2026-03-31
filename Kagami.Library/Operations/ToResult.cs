using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class ToResult : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Objects.Success => value.Just(),
      Objects.Failure => value.Just(),
      Objects.Some some => Objects.Success.Object(some.Value).Just(),
      KNil => Objects.Failure.Object("No value provided").Just(),
      _ => Objects.Success.Object(value).Just()
   };

   public override string ToString() => "to.result";
}