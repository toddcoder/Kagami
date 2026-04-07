using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class Throw : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      try
      {
         var protocol = Protocols.Protocols.GetOrThrow("PError");
         if (protocol.Supports(value))
         {
            return fail(sendMessage(value, "message".get()).AsString);
         }
         else
         {
            return fail(value.AsString);
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override string ToString() => "throw";
}