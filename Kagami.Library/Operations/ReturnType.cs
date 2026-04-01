using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using Kagami.Library.Classes;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;
using static Core.Monads.MonadFunctions;

namespace Kagami.Library.Operations;

public class ReturnType : Return
{
   protected TypeConstraint typeConstraint;

   public ReturnType(bool returnTopOfStack, TypeConstraint typeConstraint) : base(returnTopOfStack)
   {
      this.typeConstraint = typeConstraint;
   }

   public override Optional<IObject> Execute(Machine machine)
   {
      if (machine.Peek() is (true, var value))
      {
         if (convertToMonad(typeConstraint, value) is (true, var newValue))
         {
            value = newValue;
            machine.Pop();
            machine.Push(value);
         }

         var valueClass = classOf(value);
         if (valueClass is BeforeClass)
         {
            value = KBoolean.True;
            valueClass = classOf(value);
         }

         if (typeConstraint.Matches(valueClass))
         {
            return base.Execute(machine);
         }
         else
         {
            return fail($"You must return a type {typeConstraint.AsString}, not a {valueClass.Name}");
         }
      }
      else
      {
         return emptyStack("value");
      }
   }

   public override string ToString() => $"return.type({typeConstraint.AsString})";
}