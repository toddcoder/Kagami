using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public abstract class OneOperandOperation : Operation
{
   public abstract Optional<IObject> Execute(Machine machine, IObject value);

   public override Optional<IObject> Execute(Machine machine)
   {
      var _value = machine.Pop();
      if (_value is (true, var value))
      {
         return Execute(machine, value);
      }
      else
      {
         return _value.Exception;
      }
   }
}