using System;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public abstract class TwoOperandOperation : Operation
   {
      public abstract Responding<IObject> Execute(Machine machine, IObject x, IObject y);

      public override Responding<IObject> Execute(Machine machine)
      {
         try
         {
            var _y = machine.Pop();
            if (_y)
            {
               var _x = machine.Pop();
               if (_x)
               {
                  return Execute(machine, _x.Value, _y.Value);
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
}