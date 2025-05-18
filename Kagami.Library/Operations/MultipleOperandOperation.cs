using System;
using System.Collections.Generic;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public abstract class MultipleOperandOperation : Operation
{
   protected int count;

   protected MultipleOperandOperation(int count) => this.count = count;

   public abstract Result<Unit> Execute(int index, IObject value);

   public abstract Optional<IObject> Final();

   public override Optional<IObject> Execute(Machine machine)
   {
      var stack = new Stack<IObject>();

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

      var index = 0;
      while (stack.Count > 0)
      {
         var value = stack.Pop();
         var _result = Execute(index++, value);
         if (!_result)
         {
            return _result.Exception;
         }
      }

      return Final();
   }

   protected static Result<Unit> match<T>(IObject value, Action<T> action) where T : IObject
   {
      if (value is T x)
      {
         action(x);
         return unit;
      }
      else
      {
         return incompatibleClasses(value, typeof(T).Name);
      }
   }
}