using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Booleans;
using Core.Monads;
using static Kagami.Library.Operations.OperationFunctions;

namespace Kagami.Library.Operations;

public class GetIterator : OneOperandOperation
{
   protected bool lazy;

   public GetIterator(bool lazy) => this.lazy = lazy;

   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      return getIterator(value, lazy).Map(i => (IObject)i).Optional();
   }

   public override string ToString() => $"get.iterator{lazy.Extend("(lazy)")}";
}