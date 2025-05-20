using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class GoToIfTrue : AddressedOperation
{
   protected Predicate<KBoolean> predicate;

   public GoToIfTrue() => predicate = b => b.Value;

   public override Optional<IObject> Execute(Machine machine)
   {
      increment = false;

      var _x = machine.Pop();
      if (_x is (true, var x))
      {
         if (x is KBoolean bx)
         {
            if (predicate(bx))
            {
               return machine.GoTo(address) ? nil : badAddress(address);
            }

            increment = true;
            return nil;
         }
         else
         {
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