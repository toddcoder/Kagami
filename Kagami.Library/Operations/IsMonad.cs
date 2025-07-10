using Core.Monads;
using Core.Numbers;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class IsMonad(MonadType monad) : OneOperandOperation
{
   protected Bits32<MonadType> monadType = monad;

   public override Optional<IObject> Execute(Machine machine, IObject value) => monadType[MonadType.Some] && value is Objects.Some ||
      monadType[MonadType.None] && value is KNil ||
      monadType[MonadType.Success] && value is Objects.Success || monadType[MonadType.Failure] && value is Objects.Failure ? KBoolean.True.Just()
         : KBoolean.False.Just();

   public override string ToString() => $"is.monad({monadType})";
}