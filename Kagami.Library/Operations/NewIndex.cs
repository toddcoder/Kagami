using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class NewIndex : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      if (x is Int xInt)
      {
         if (y is Int yInt)
         {
            return KIndex.New(xInt.Value, yInt.Value).Just();
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

   public override string ToString() => "new.index";
}