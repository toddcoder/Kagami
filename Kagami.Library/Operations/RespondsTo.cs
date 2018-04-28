using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Maybe;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Standard.Types.Maybe.MaybeFunctions;

namespace Kagami.Library.Operations
{
   public class RespondsTo : Operation
   {
      string message;

      public RespondsTo(string message) => this.message = message;

      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.Peek().If(out var value))
            return Boolean.Object(classOf(value).RespondsTo(message)).Matched();
         else
            return failedMatch<IObject>(emptyStack());
      }

      public override string ToString() => "responds.to";
   }
}