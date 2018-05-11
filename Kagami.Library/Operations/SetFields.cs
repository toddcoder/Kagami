using Kagami.Library.Invokables;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class SetFields : Operation
   {
      Parameters parameters;

      public SetFields(Parameters parameters) => this.parameters = parameters;

      public override IMatched<IObject> Execute(Machine machine)
      {
         var frame = machine.CurrentFrame;
         frame.SetFields(parameters);

         return notMatched<IObject>();
      }

      public override string ToString() => $"set.fields({parameters})";
   }
}