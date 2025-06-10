using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class PushDecimal(decimal value) : Operation
{
   protected XDecimal value = new(value);

   public override Optional<IObject> Execute(Machine machine) => value;

   public override string ToString() => $"push.decimal({value.Image})";
}