using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class ReadLine : Operation
{
   public override Optional<IObject> Execute(Machine machine)
   {
      if (machine.Context.ReadLine() is (true, var line))
      {
         return String.StringObject(line).Just();
      }

      machine.Running = false;
      return String.Empty.Just();
   }

   public override string ToString() => "readln";
}