using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class NewMutString(string text) : Operation
{
   public override Optional<IObject> Execute(Machine machine) => new MutString(text);

   public override string ToString() => "new.mut.string";
}