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
         return KString.StringObject(line).Just();
      }

      machine.Running = false;
      return KString.Empty.Just();
   }

   public override string ToString() => "readln";
}