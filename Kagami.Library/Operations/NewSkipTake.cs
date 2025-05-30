using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewSkipTake : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      if (x is Int skip)
      {
         if (y is Int take)
         {
            return new Objects.SkipTake(skip.Value, take.Value).Just<IObject>();
         }
         else
         {
            return fail("Expected Int take value");
         }
      }
      else
      {
         return fail("Expected Int skip value");
      }
   }

   public override string ToString() => "new.skip.take";
}