using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public abstract class TwoOperandOperation : Operation
{
   public abstract Optional<IObject> Execute(Machine machine, IObject x, IObject y);

   public override Optional<IObject> Execute(Machine machine)
   {
      try
      {
         var _y = machine.Pop();
         if (_y is (true, var y))
         {
            var _x = machine.Pop();
            if (_x is (true, var x))
            {
               return Execute(machine, x, y);
            }
            else
            {
               return _x.Exception;
            }
         }
         else
         {
            return _y.Exception;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}