using System;
using System.Collections.Generic;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations
{
   public abstract class MultipleOperandOperation : Operation
   {
      protected int count;

      protected MultipleOperandOperation(int count) => this.count = count;

      public abstract IResult<Unit> Execute(Machine machine, int index, IObject value);

      public abstract IMatched<IObject> Final(Machine machine);

      public override IMatched<IObject> Execute(Machine machine)
      {
         var stack = new Stack<IObject>();

         for (var i = 0; i < count; i++)
         {
	         if (machine.Pop().If(out var obj, out var exception))
	         {
		         stack.Push(obj);
	         }
	         else
	         {
		         return failedMatch<IObject>(exception);
	         }
         }

         var index = 0;
         while (stack.Count > 0)
         {
            var value = stack.Pop();
            if (Execute(machine, index++, value).If(out _, out var exception)) { }
            else
            {
	            return failedMatch<IObject>(exception);
            }
         }

         return Final(machine);
      }

      protected static IResult<Unit> match<T>(IObject value, Action<T> action) where T : IObject
      {
         if (value is T x)
         {
            action(x);
            return Unit.Success();
         }
         else
         {
	         return failure<Unit>(incompatibleClasses(value, typeof(T).Name));
         }
      }
   }
}