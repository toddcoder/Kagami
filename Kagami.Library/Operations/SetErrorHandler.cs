using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class SetErrorHandler : AddressedOperation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      increment = true;
      var _errorHandler = machine.SetErrorHandler(address);
      if (_errorHandler)
      {
         return nil;
      }
      else
      {
         return _errorHandler.Exception;
      }
   }

   public override string ToString() => "set.error.handler";
}