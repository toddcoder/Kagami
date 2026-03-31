using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class ToOptional : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Objects.Some or KNil => value.Just(),
      Objects.Success success => Objects.Some.Object(success.Value).Just(),
      Objects.Failure => KNil.NilValue.Just(),
      _ => Objects.Some.Object(value).Just()
   };

   public override string ToString() => "to.optional";
}