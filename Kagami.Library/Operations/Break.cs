using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Break : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      machine.Clear();
      return nil;
   }

   public override string ToString() => "break";
}