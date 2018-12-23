using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Booleans;
using Standard.Types.Monads;
using static Kagami.Library.Operations.OperationFunctions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class GetIterator : OneOperandOperation
   {
      bool lazy;

      public GetIterator(bool lazy) => this.lazy = lazy;

      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         return getIterator(value, lazy).Map(i => (IObject)i).FlatMap(o => o.Matched(), failedMatch<IObject>);
      }

      public override string ToString() => $"get.iterator{lazy.Extend("(lazy)")}";
   }
}