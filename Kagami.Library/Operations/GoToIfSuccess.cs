using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class GoToIfSuccess : AddressedOperation
{
   protected Predicate<IResult> predicate = o => o.IsSuccess;
   protected Func<IObject, Optional<IObject>> returnIfTrue = s => s is Objects.Success success ? success.Value.Just() : nil;
   protected Func<IObject, Optional<IObject>> returnIfFalse = s => s is Objects.Success success ? success.Value.Just() : nil;

   public override Optional<IObject> Execute(Machine machine)
   {
      increment = false;

      var _value = machine.Pop();
      if (_value is (true, var value))
      {
         switch (value)
         {
            case IResult o when predicate(o):
               return machine.GoTo(address) ? returnIfTrue(value) : badAddress(address);

            case IResult:
               increment = true;
               return returnIfFalse(value);
            default:
               return incompatibleClasses(value, "Result");
         }
      }
      else
      {
         return _value.Exception;
      }
   }

   public override string ToString() => "goto.if.success";
}