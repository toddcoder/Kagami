using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class PushFloat : Operation
   {
      Float value;

      public PushFloat(double value) => this.value = value;

      public override IMatched<IObject> Execute(Machine machine) => value.Matched<IObject>();

      public override string ToString() => $"push.float({value.Image})";
   }
}