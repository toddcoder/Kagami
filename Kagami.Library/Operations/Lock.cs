using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class Lock : OneOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject value)
   {
      switch (value)
      {
         case IMutable mutable:
            mutable.IsLocked = true;
            return value.Just();
         case IHasReadOnlyFriend hasReadOnlyFriend:
            return hasReadOnlyFriend.ReadOnlyFriend.Just();
         default:
            return objectIsNotLockable(value);
      }
   }

   public override string ToString() => "lock";
}