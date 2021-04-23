using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class PushByte : Operation
   {
      protected Byte value;

      public PushByte(byte value) => this.value = value;

      public override IMatched<IObject> Execute(Machine machine) => value.Matched<IObject>();

      public override string ToString() => $"push.byte({value.Image})";
   }
}