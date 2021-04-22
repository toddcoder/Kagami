using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;
using Boolean = Kagami.Library.Objects.Boolean;

namespace Kagami.Library.Operations
{
   public class GoToIfTrue : AddressedOperation
   {
      protected Predicate<Boolean> predicate;

      public GoToIfTrue() => predicate = b => b.Value;

      public override IMatched<IObject> Execute(Machine machine)
      {
         increment = false;

         if (machine.Pop().If(out var x, out var exception))
         {
            if (x is Boolean bx)
            {
               if (predicate(bx))
               {
                  return machine.GoTo(address) ? notMatched<IObject>() : failedMatch<IObject>(badAddress(address));
               }

               increment = true;
               return notMatched<IObject>();
            }
            else
            {
               return failedMatch<IObject>(incompatibleClasses(x, "Boolean"));
            }
         }
         else
         {
            return failedMatch<IObject>(exception);
         }
      }

      public override string ToString() => $"goto.if.true({address})";
   }
}