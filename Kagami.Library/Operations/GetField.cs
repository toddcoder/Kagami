using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class GetField : Operation
   {
      string fieldName;

      public GetField(string fieldName) => this.fieldName = fieldName;

      public override IMatched<IObject> Execute(Machine machine)
      {
         return machine.Find(fieldName, true).FlatMap(f => f.Value.Matched(), () => failedMatch<IObject>(fieldNotFound(fieldName)),
            failedMatch<IObject>);
      }

      public override string ToString() => $"get.field({fieldName})";
   }
}