using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Kagami.Library.AllExceptions;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class GoToIfSome : AddressedOperation
   {
      protected Predicate<IOptional> predicate;
      protected Func<IObject, IMatched<IObject>> returnIfTrue;
      protected Func<IObject, IMatched<IObject>> returnIfFalse;

      public GoToIfSome()
      {
         predicate = o => o.IsSome;
         returnIfTrue = s => s is Objects.Some some ? some.Value.Matched() : notMatched<IObject>();
         returnIfFalse = s => s is Objects.Some some ? some.Value.Matched() : notMatched<IObject>();
      }

      public override IMatched<IObject> Execute(Machine machine)
      {
         increment = false;

         if (machine.Pop().If(out var value, out var exception))
            switch (value)
            {
               case IOptional o when predicate(o):
                  if (machine.GoTo(address))
                     return returnIfTrue(value);
                  else
                     return failedMatch<IObject>(badAddress(address));
               case IOptional _:
                  increment = true;
                  return returnIfFalse(value);
               default:
                  return failedMatch<IObject>(incompatibleClasses(value, "Optional"));
            }
         else
            return failedMatch<IObject>(exception);
      }

      public override string ToString() => "goto.if.some";
   }
}