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
         /*if (typeConstraint.ConvertToMonad(value) is (true, var monad))
         {
            value = monad;
            machine.Pop();
            machine.Push(value);
         }*/

         var valueClass = classOf(value);
         if (valueClass is BeforeClass)
         {
            value = KBoolean.True;
            valueClass = classOf(value);
         }

         /*if (typeConstraint.Matches(valueClass))
         {
            return base.Execute(machine);
         }
         else
         {
            return fail($"You must return a type {typeConstraint.AsString}, not a {valueClass.Name}");
         }*/
         var _result = Field.GetFieldValue(value, typeConstraint);
         if (_result is (true, var (newValue, _)))
         {
            machine.Pop();
            machine.Push(newValue);
            return base.Execute(machine);
         }
         else
         {
            throw _result.Exception;
         }
      }
      else
      {
         return emptyStack("value");
      }
   }

   public override string ToString() => $"return.type({typeConstraint.AsString})";
}