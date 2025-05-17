using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public abstract class TwoIntOperation : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      if (x is Int xInt)
      {
         if (y is Int yInt)
         {
            return Execute(xInt.Value, yInt.Value).Map(Int.IntObject);
         }
         else
         {
            return incompatibleClasses(y, "Int");
         }
      }
      else
      {
         return incompatibleClasses(x, "Int");
      }
   }

   public abstract Optional<int> Execute(int x, int y);
}