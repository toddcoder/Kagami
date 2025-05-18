using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public abstract class TwoNumericOperation : Operation
{
   public abstract Optional<IObject> Execute(INumeric x, INumeric y);

   public override Optional<IObject> Execute(Machine machine)
   {
      try
      {
         var _xy =
            from yValue in machine.Pop()
            from xValue in machine.Pop()
            select (xValue, yValue);
         if (_xy is (true, var (x, y)))
         {
            if (x is INumeric nx)
            {
               if (y is INumeric ny)
               {
                  return Execute(nx, ny);
               }
               else
               {
                  return notNumeric(y.Image);
               }
            }
            else
            {
               return notNumeric(x.Image);
            }
         }
         else
         {
            return _xy.Exception;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}