using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
   public class PushInt : Operation
   {
      Int value;

      public PushInt(int value) => this.value = value;

      public override IMatched<IObject> Execute(Machine machine) => value.Matched<IObject>();

      public override string ToString() => $"push.int({value.Image})";
   }
}