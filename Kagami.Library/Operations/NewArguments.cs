using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class NewArguments : OneNumericOperation
{
   public override Optional<IObject> Execute(Machine machine, INumeric x)
   {
      var count = x.AsInt32();
      Stack<IObject> stack = [];
      for (var i = 0; i < count; i++)
      {
         var _obj = machine.Pop();
         if (_obj is (true, var obj))
         {
            stack.Push(obj);
         }
         else
         {
            return fail($"Too few arguments: expected {count}, found {stack.Count}");
         }
      }

      IObject[] array = [.. stack];
      var arguments = new Arguments(array);

      return arguments;
   }

   public override string ToString() => "new.arguments";
}