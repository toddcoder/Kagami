using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
   public class PushBoolean : Operation
   {
      Boolean value;

      public PushBoolean(bool value) => this.value = value;

      public override IMatched<IObject> Execute(Machine machine) => value.Matched<IObject>();

      public override string ToString() => $"push.boolean({value.Image})";
   }
}