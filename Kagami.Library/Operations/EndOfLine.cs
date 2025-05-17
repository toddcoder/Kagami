using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class EndOfLine : Operation
{
   public override Optional<IObject> Execute(Machine machine) => nil;

   public override string ToString() => "end.of.line";
}