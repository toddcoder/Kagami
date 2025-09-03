using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class GoToIfTrue : AddressedOperation
{
   protected Predicate<IBoolean> predicate = b => b.IsTrue;

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
            case IBoolean:
               increment = true;
               return nil;
            case Before:
               return machine.GoTo(address) ? nil : badAddress(address);
            case Junction junction:
            {
               return KBoolean.BooleanObject(junction.IsTrue).Just();
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

   public override string ToString() => $"goto.if.true({address})";
}