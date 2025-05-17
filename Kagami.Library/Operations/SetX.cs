using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class SetX : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      machine.X = value;
      return nil;
   }

   public override string ToString() => "set.x";
}