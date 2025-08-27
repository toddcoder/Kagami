using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class Roll : Operation
{
   protected int count;

   public Roll(int count) => this.count = count;

   public override Optional<IObject> Execute(Machine machine)
   {
      Stack<IObject> stack = [];
      for (var i = 0; i < count; i++)
      {
         var _value = machine.Pop();
         if (_value is (true, var value))
         {
            stack.Push(value);
         }
         else
         {
            return _value.Exception;
         }
      }

      while (stack.Count > 0)
      {
         machine.Push(stack.Pop());
      }

      return nil;
   }

   public override string ToString() => $"roll({count})";
}