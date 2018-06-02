using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class Peek : Operation
   {
      int index;

      public Peek(int index) => this.index = index;

      public override string ToString() => $"peek({index})";

      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.CurrentFrame.Peek().If(out var value))
         {
            var message = $"{value.Image} | {value.ClassName}";
            machine.Context.Peek(message, index);
         }

         return notMatched<IObject>();
      }
   }
}