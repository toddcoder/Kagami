using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class PushString : Operation
{
   protected KString value;

   public PushString(string value) => this.value = value;

   public override Optional<IObject> Execute(Machine machine) => value;

   public override string ToString() => $"push.string({value.Image})";
}