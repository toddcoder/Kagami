using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class NameOf(string name) : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      if (char.IsUpper(name[0]))
      {
         var _class = Module.Global.Value.Class(name);
         if (_class)
         {
            return KString.StringObject(name).Just();
         }
      }

      var _field = machine.Find(name, true);
      if (_field)
      {
         return KString.StringObject(name).Just();
      }
      else if (_field.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return fieldNotFound(name);
      }
   }

   public override string ToString() => "name.of";
}