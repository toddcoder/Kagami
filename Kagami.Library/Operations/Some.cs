using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class Some : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Objects.Success success => Objects.Some.Object(success.Value, TypeConstraint.SingleType(success.Value.ClassName)).Just(),
      KNil => value.Just(),
      Objects.Failure => KNil.NilValue.Just(),
      _ => Objects.Some.Object(value, TypeConstraint.SingleType(value.ClassName)).Just()
   };

   public override string ToString() => "some";
}