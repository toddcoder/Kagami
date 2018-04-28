using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;

namespace Kagami.Library.Operations
{
   public class Equal : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y) => Boolean.Object(x.IsEqualTo(y)).Matched();

      public override string ToString() => "equal";
   }
}