using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class UnwrapMonad : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => value switch
   {
      Objects.Some some => some.Value.Just(),
      Objects.Success success => success.Value.Just(),
      _ => value.Just()
   };

   public override string ToString() => "unwrap.monad";
}