using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class GoToIfFailure : AddressedOperation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      increment = false;

      if (machine.Peek() is (true, var value))
      {
         if (value is IResult result)
         {
            if (result.IsFailure)
            {
               machine.GoTo(address);
            }
            else
            {
               increment = true;
            }
         }
         else
         {
            increment = true;
         }

         return nil;
      }
      else
      {
         return emptyStack("value");
      }
   }

   public override string ToString() => "goto.if.failure";
}