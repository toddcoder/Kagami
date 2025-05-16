using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class PushBoolean : Operation
{
   protected Boolean value;

   public PushBoolean(bool value) => this.value = value;

   public override Optional<IObject> Execute(Machine machine) => value;

   public override string ToString() => $"push.boolean({value.Image})";
}