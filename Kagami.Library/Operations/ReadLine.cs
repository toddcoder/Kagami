using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations
{
   public class ReadLine : Operation
   {
      public override IMatched<IObject> Execute(Machine machine)
      {
         if (machine.Context.ReadLine().If(out var line))
            return String.StringObject(line).Matched();

         machine.Running = false;
         return String.Empty.Matched();
      }

      public override string ToString() => "readln";
   }
}