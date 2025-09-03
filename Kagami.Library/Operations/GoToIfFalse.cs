using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class GoToIfFalse : AddressedOperation
{
   protected Predicate<IBoolean> predicate = b => !b.IsTrue;

   public override string ToString() => $"goto.if.false({address})";

   public override Optional<IObject> Execute(Machine machine)
   {
      increment = false;

      var _x = machine.Pop();
      if (_x is (true, var x))
      {
         switch (x)
         {
            case IBoolean bx when predicate(bx):
               return machine.GoTo(address) ? nil : badAddress(address);
            case IBoolean or Before:
               increment = true;
               return nil;
            case Junction junction:
            {
               return KBoolean.BooleanObject(!junction.IsTrue).Just();
            }
            default:
               return incompatibleClasses(x, "Boolean");
         }
      }
      else
      {
         return _x.Exception;
      }
   }
}