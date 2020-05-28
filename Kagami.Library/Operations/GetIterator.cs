using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Booleans;
using Core.Monads;
using static Kagami.Library.Operations.OperationFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class GetIterator : OneOperandOperation
   {
      bool lazy;

      public GetIterator(bool lazy) => this.lazy = lazy;

      public override IMatched<IObject> Execute(Machine machine, IObject value)
      {
         return getIterator(value, lazy).Map(i => (IObject)i).Map(o => o.Matched()).Recover(failedMatch<IObject>);
      }

      public override string ToString() => $"get.iterator{lazy.Extend("(lazy)")}";
   }
}