using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class Failure : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      try
      {
         var protocol = Protocols.Protocols.GetOrThrow("PError");
         if (protocol.Supports(value))
         {
            return new Objects.Failure(value);
         }
         else
         {
            return Objects.Failure.Object(value.AsString).Just();
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public override string ToString() => "failure";
}