using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class AsString : OneOperandOperation
   {
      public override IMatched<IObject> Execute(Machine machine, IObject value) => String.StringObject(value.AsString).Matched();

      public override string ToString() => "string";
   }
}