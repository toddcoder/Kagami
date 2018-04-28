using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class PushChar : Operation
   {
      Char value;

      public PushChar(char value) => this.value = value;

      public override IMatched<IObject> Execute(Machine machine) => value.Matched<IObject>();

      public override string ToString() => $"push.char({value.Image})";
   }
}