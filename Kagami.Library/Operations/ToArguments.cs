using System.Collections.Generic;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;

namespace Kagami.Library.Operations;

public class ToArguments : OneNumericOperation
{
   public override Optional<IObject> Execute(Machine machine, INumeric x)
   {
      var count = x.AsInt32();
      var stack = new Stack<IObject>();
      for (var i = 0; i < count; i++)
      {
         var _obj = machine.Pop();
         if (_obj is (true, var obj))
         {
            stack.Push(obj);
         }
         else
         {
            return _obj.Exception;
         }
      }

      var array = stack.ToArray();
      var arguments = new Arguments(array);

      return arguments;
   }

   public override string ToString() => "to.arguments";
}