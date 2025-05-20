using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class PushByte : Operation
{
   protected KByte value;

   public PushByte(byte value) => this.value = value;

   public override Optional<IObject> Execute(Machine machine) => value;

   public override string ToString() => $"push.byte({value.Image})";
}