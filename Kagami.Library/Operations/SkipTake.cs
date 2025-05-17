using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class SkipTake : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      if (x is Int xInt)
      {
         return y switch
         {
            Int yInt => new Objects.SkipTake(xInt.Value, yInt.Value),
            None => new Objects.SkipTake(xInt.Value, 0) { NoTake = true },
            _ => incompatibleClasses(y, "Int")
         };
      }
      else
      {
         return incompatibleClasses(x, "Int");
      }
   }

   public override string ToString() => "skip.take";
}