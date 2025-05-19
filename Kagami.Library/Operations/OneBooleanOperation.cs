using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public abstract class OneBooleanOperation : Operation
{
   public abstract Optional<bool> Execute(bool boolean);

   public override Optional<IObject> Execute(Machine machine)
   {
      var _value = machine.Pop();
      if (_value is (true, var value))
      {
         if (value is Boolean b)
         {
            return Execute(b.Value).Map(Boolean.BooleanObject);
         }
         else
         {
            return incompatibleClasses(value, "Boolean");
         }
      }
      else
      {
         return _value.Exception;
      }
   }
}