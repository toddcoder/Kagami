using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class Some : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => Objects.Some.Object(value).Matched();

      public override string ToString() => "some";
   }
}