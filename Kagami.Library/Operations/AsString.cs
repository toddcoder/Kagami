using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class AsString : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => String.Object(value.AsString).Matched();

      public override string ToString() => "string";
   }
}