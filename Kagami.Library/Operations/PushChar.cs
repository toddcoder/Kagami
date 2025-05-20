using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class PushChar : Operation
{
   protected KChar value;

   public PushChar(char value) => this.value = value;

   public override Optional<IObject> Execute(Machine machine) => value;

   public override string ToString() => $"push.char({value.Image})";
}