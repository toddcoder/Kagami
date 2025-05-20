using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Booleans;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class IsClass : Operation
{
   protected static Optional<IObject> getValue(bool pop, Machine machine, string className)
   {
      if (pop)
      {
         var _value = machine.Pop();
         if (_value is (true, var value))
         {
            return value.Just();
         }
         else
         {
            return _value.Exception;
         }
      }
      else
      {
         var _value = machine.Peek();
         if (_value is (true, var value))
         {
            return value.Just();
         }
         else
         {
            return fail($"Couldn't peek value to determine class {className}");
         }
      }
   }

   protected string className;
   protected bool pop;

   public IsClass(string className, bool pop)
   {
      this.className = className;
      this.pop = pop;
   }

   public override Optional<IObject> Execute(Machine machine)
   {
      return getValue(pop, machine, className).Map(value => KBoolean.BooleanObject(value.ClassName == className));
   }

   public override string ToString() => $"is.class({className}{pop.Extend(", pop")})";
}