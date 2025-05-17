using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public abstract class ArgumentsOperation : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      var _value = machine.Pop();
      if (_value is (true, var value))
      {
         if (value is Arguments arguments)
         {
            return Execute(arguments);
         }
         else
         {
            return incompatibleClasses(value, "Arguments");
         }
      }
      else
      {
         return _value.Exception;
      }
   }

   public abstract Optional<IObject> Execute(Arguments arguments);
}