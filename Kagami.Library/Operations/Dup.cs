using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;

namespace Kagami.Library.Operations;

public class Dup : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      if (machine.Peek() is (true, var value))
      {
         return value.Just();
      }
      else
      {
         return requiresNOperands(1);
      }
   }

   public override string ToString() => "dup";
}