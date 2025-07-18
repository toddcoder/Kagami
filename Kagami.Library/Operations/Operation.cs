using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;
using Machine = Kagami.Library.Runtime.Machine;

namespace Kagami.Library.Operations;

public abstract class Operation
{
   public abstract Optional<IObject> Execute(Machine machine);

   protected Optional<(IObject x, IObject y)> implicitConversion(Machine machine, IObject x, IObject y)
   {
      var fromClass = x.ClassName;
      var toClass = y.ClassName;

      var _selector = Module.Global.Value.GetConversion(fromClass, toClass);
      if (_selector is (true, var selector))
      {
         var _field = machine.Find(selector);
         if (_field is (true, var selectedField))
         {
            return Invoke.InvokeObject(machine, selectedField.Value, arguments, ref increment);
         }
         else if (_field.Exception is (true, var exception))
         {
            return exception;
         }
         else
         {
            return fieldNotFound(image);
         }
      }
      else
      {
         return (x, y);
      }
   }

   public virtual bool Increment => true;
}