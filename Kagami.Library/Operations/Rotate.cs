using System.Collections.Generic;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Standard.Types.Monads;
using static Standard.Types.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class Rotate : Operation
   {
      int count;

      public Rotate(int count) => this.count = count;

      public override IMatched<IObject> Execute(Machine machine)
      {
         var list = new List<IObject>();
         for (var i = 0; i < count; i++)
            if (machine.Pop().If(out var value, out var exception))
               list.Add(value);
            else
               return failedMatch<IObject>(exception);

         foreach (var value in list)
            machine.Push(value);

         return notMatched<IObject>();
      }

      public override string ToString() => $"rotate({count})";
   }
}