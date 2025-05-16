using System.Linq;
using Kagami.Library.Objects;
using Kagami.Library.Runtime;
using Core.Monads;
using static Kagami.Library.AllExceptions;
using static Kagami.Library.Objects.ObjectFunctions;

namespace Kagami.Library.Operations;

public class Pipeline : TwoOperandOperation
{
   public override Optional<IObject> Execute(Machine machine, IObject x, IObject y)
   {
      switch (y)
      {
         case Lambda lambda:
            if (x is ICollection collection)
            {
               var array = collection.GetIterator(false).List().ToArray();
               return lambda.Invoke(array).Just();
            }
            else
            {
               return lambda.Invoke(x).Just();
            }

         case IMayInvoke mi:
            return mi.Invoke(x).Just();
         case Message message:
            return classOf(x).SendMessage(x, message).Just();
         case Selector selector:
            if (Machine.Current.Find(selector).If(out var field, out var anyException))
            {
               var _ = false;
               return Invoke.InvokeObject(machine, field.Value, new Arguments(x), ref _).Just();
            }
            else if (anyException.If(out var exception))
            {
               return exception;
            }
            else
            {
               return fieldNotFound(selector);
            }

         default:
            return incompatibleClasses(y, "Lambda");
      }
   }

   public override string ToString() => "pipeline";
}