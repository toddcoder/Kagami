using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewSkipTake : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value) => nil; //new SkipTake(value).Matched<IObject>();

   public override string ToString() => "new.skip.take";
}