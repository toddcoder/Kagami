using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class GoToIfSome : AddressedOperation
{
   protected Predicate<IOptional> predicate;
   protected Func<IObject, Optional<IObject>> returnIfTrue;
   protected Func<IObject, Optional<IObject>> returnIfFalse;

   public GoToIfSome()
   {
      predicate = o => o.IsSome;
      returnIfTrue = s => s is Objects.Some some ? some.Value.Just() : nil;
      returnIfFalse = s => s is Objects.Some some ? some.Value.Just() : nil;
   }

   public override Optional<IObject> Execute(Machine machine)
   {
      increment = false;

      var _value = machine.Pop();
      if (_value is (true, var value))
      {
         switch (value)
         {
            case IOptional o when predicate(o):
               return machine.GoTo(address) ? returnIfTrue(value) : badAddress(address);

            case IOptional:
               increment = true;
               return returnIfFalse(value);
            default:
               return incompatibleClasses(value, "Optional");
         }
      }
      else
      {
         return _value.Exception;
      }
   }

   public override string ToString() => "goto.if.some";
}