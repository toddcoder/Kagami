using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class FieldExists : Operation
   {
      protected string fieldName;

      public FieldExists(string fieldName) => this.fieldName = fieldName;

      public override IMatched<IObject> Execute(Machine machine)
      {
         return machine.Find(fieldName, true).FlatMap(_ => Boolean.True.Matched(), () => Boolean.False.Matched(), failedMatch<IObject>);
      }

      public override string ToString() => $"field.exists({fieldName})";
   }
}