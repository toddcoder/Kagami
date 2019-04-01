using System.Collections.Generic;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public class ToArguments : OneNumericOperation
   {
      public override IMatched<IObject> Execute(Machine machine, INumeric x)
      {
         var count = x.AsInt32();
         var stack = new Stack<IObject>();
         for (var i = 0; i < count; i++)
            if (machine.Pop().If(out var obj, out var exception))
               stack.Push(obj);
            else
               return failedMatch<IObject>(exception);

         var array = stack.ToArray();
         var arguments = new Arguments(array);

         return arguments.Matched<IObject>();
      }

      public override string ToString() => "to.arguments";
   }
}