using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class Success : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Objects.Some some => Objects.Success.Object(some.Value).Just(),
      KNil => Objects.Failure.Object("Nil value").Just(),
      Objects.Failure => value.Just(),
      _ => Objects.Success.Object(value).Just()
   };

   public override string ToString() => "success";
}