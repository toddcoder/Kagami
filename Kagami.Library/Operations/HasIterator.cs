using Core.Monads;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;

namespace Kagami.Library.Operations;

public class HasIterator : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      if (machine.Peek() is (true, var value))
      {
         return KBoolean.BooleanObject(value is ICollection).Just();
      }
      else
      {
         return KBoolean.False.Just();
      }
   }

   public override string ToString() => "has.iterator";
}