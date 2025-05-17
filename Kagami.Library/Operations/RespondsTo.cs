using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class RespondsTo : Operation
{
   protected string message;

   public RespondsTo(string message) => this.message = message;

   public override Optional<IObject> Execute(Machine machine)
   {
      if (machine.Peek() is (true, var value))
      {
         return Boolean.BooleanObject(classOf(value).RespondsTo(message)).Just();
      }
      else
      {
         return emptyStack();
      }
   }

   public override string ToString() => "responds.to";
}