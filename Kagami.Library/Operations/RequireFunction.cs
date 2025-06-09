using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class RequireFunction(Selector selector) : Operation
{
   public override Optional<IObject> Execute(Machine machine) => fail($"Function {selector} not implemented");

   public override string ToString() => "require.function";
}