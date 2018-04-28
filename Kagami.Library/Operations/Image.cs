using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class Image : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => String.Object(value.Image).Matched();

      public override string ToString() => "image";
   }
}