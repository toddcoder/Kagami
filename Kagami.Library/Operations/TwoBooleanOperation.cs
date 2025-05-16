using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public abstract class TwoBooleanOperation : Operation
{
   public abstract Optional<bool> Execute(bool x, bool y);

   public override Optional<IObject> Execute(Machine machine)
   {
      var _xy =
         from yValue in machine.Pop()
         from xValue in machine.Pop()
         select (xValue, yValue);
      if (_xy is (true, var (x, y)))
      {
         if (x is Boolean bx)
         {
            if (y is Boolean by)
            {
               return Execute(bx.Value, by.Value).Map(Boolean.BooleanObject);
            }
            else
            {
               return incompatibleClasses(y, "Boolean");
            }
         }
         else
         {
            return incompatibleClasses(x, "Boolean");
         }
      }
      else
      {
         return _xy.Exception;
      }
   }
}