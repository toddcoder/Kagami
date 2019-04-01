using System.Collections.Generic;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Roll : Operation
   {
      int count;

      public Roll(int count) => this.count = count;

      public override IMatched<IObject> Execute(Machine machine)
      {
         var stack = new Stack<IObject>();
         for (var i = 0; i < count; i++)
            if (machine.Pop().If(out var value, out var exception))
               stack.Push(value);
            else
               return failedMatch<IObject>(exception);

         while (stack.Count > 0)
            machine.Push(stack.Pop());

         return notMatched<IObject>();
      }

      public override string ToString() => $"roll({count})";
   }
}