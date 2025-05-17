using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
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
         var valueClass = classOf(value);
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
         return emptyStack();
      }
   }

   public override string ToString() => $"return.type({typeConstraint.AsString})";
}