using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;

namespace Kagami.Library.Operations
{
   public class NewKeyValue : TwoOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject x, IObject y) => new KeyValue(x, y).Matched<IObject>();

      public override string ToString() => "new.key.value";
   }
}